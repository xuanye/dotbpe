using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PiggyMetrics.Common;

namespace PiggyMetrics.HttpApi
{
    public class ForwardService : IForwardService
    {
        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<ForwardService>();
        private readonly AmpCommonClient _client;
        private readonly RouterOption _option;

        public ForwardService(IRpcClient<AmpMessage> rpcCleint,IOptionsSnapshot<RouterOption> optionsAccessor){
            Assert.IsNull(optionsAccessor.Value,"RouterOption not found");

            _option = optionsAccessor.Value;

            _client = new AmpCommonClient(rpcCleint);
        }

        public Task<CallResult> ForwardAysnc(HttpContext context)
        {

            string path = context.Request.Path;
            string method =  context.Request.Method;

            RouterData rd = null;
            try
            {
                 rd = FindRouter(path, method, context);
            }
            catch(Exception ex)
            {
                CallResult result = new CallResult()
                {
                    Status = 500,
                    Message = "Error Request"+ex.Message
                };
                return Task.FromResult(result);
            }
            if(rd == null)
            {
                CallResult result = new CallResult()
                {
                    Status = 404,
                    Message = "service not found!"
                };

                return Task.FromResult(result);

            }
            if(rd.NeedAuth && !context.User.Identity.IsAuthenticated)
            {
                CallResult result = new CallResult(){
                    Status = 501,
                    Message = "Need Authenticate"
                };
                return Task.FromResult(result);
            }

            return this._client.SendAsync((ushort)rd.ServiceId,(ushort)rd.MessageId,rd.Body,rd.Data);


        }



        private RouterData FindRouter(string path, string method,HttpContext context)
        {
            for(var i=0; i<_option.Routers.Count ;i++){
                var router = _option.Routers[i];
                if(router.Method.Equals(method,StringComparison.OrdinalIgnoreCase)){
                   var match =  Match(router.Path,path);
                   if(match.Success){
                       RouterData rd = new RouterData();
                       rd.ServiceId = router.ServiceId;
                       rd.MessageId = router.MessageId;
                       rd.NeedAuth = router.NeedAuth;
                       rd.Data = new Dictionary<string,string>();
                       if( match.Groups !=null && match.Groups.Count>0){
                          CollectParams(match.Groups,rd.Data);
                       }
                       CollectQuery(context.Request.Query,rd.Data);
                       string contentType = "";
                       if (method.ToLower() =="post" || method.ToLower()=="put")
                       {
                            contentType = context.Request.ContentType.ToLower().Split(';')[0];
                       }

                       if (contentType == "application/x-www-form-urlencoded"
                            || contentType == "multipart/form-data"
                            )
                       {
                            CollectForm(context.Request.Form, rd.Data);
                       }

                       if(contentType == "application/json")
                        {
                            rd.Body = CollectBody(context.Request.Body);
                        }

                       if(context.User.Identity.IsAuthenticated){
                            //添加当前用户; 实际的项目中可根据自己的情况去扩展
                            rd.Data.Add("current",context.User.Identity.Name);
                       }
                       //添加客户端IP 项目中可根据实际情况添加需要的内容
                       var IPAddress = context.Connection.RemoteIpAddress;
                       string ip =  IPAddress.IsIPv4MappedToIPv6?IPAddress.MapToIPv4().ToString():IPAddress.ToString();
                       rd.Data.Add("clientip",ip);
                       return rd;
                   }
                }
            }

            return null;

        }

        private string CollectBody(Stream body)
        {
            string bodyText = null;
            using (StreamReader reader = new StreamReader(body))
            {
                bodyText = reader.ReadToEnd();
            }
            return bodyText;
        }

        private void CollectForm(IFormCollection form,Dictionary<string,string> routeData)
        {
            foreach (string key in form.Keys){
                if( routeData.ContainsKey(key))
                    routeData[key] = form[key];
                else
                    routeData.Add(key,form[key]);
            }
        }

        private void CollectQuery(IQueryCollection query,Dictionary<string,string> routeData)
        {
            foreach (string key in query.Keys){
                if( routeData.ContainsKey(key))
                    routeData[key] = query[key];
                else
                    routeData.Add(key,query[key]);
            }

        }

        private void CollectParams(GroupCollection groups,Dictionary<string,string> routeData)
        {

            foreach (Group group in groups){
                if( routeData.ContainsKey(group.Name))
                    routeData[group.Name] = group.Value ;
                else
                    routeData.Add(group.Name,group.Value);
            }
        }

        private Match Match(string regex ,string value){
            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            return reg.Match(value);
        }
    }


    public class RouterData{
        public int ServiceId{get;set;}
        public int MessageId {get;set;}
        public string Body { get; set; }
        public Dictionary<string,string> Data{get;set;}

        public bool NeedAuth{get;set;}
    }
}
