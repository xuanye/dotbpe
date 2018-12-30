using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Server.Impl
{
    public class BaseService<TInterFace> : AbsServiceActor where TInterFace : class
    {
        static ConcurrentDictionary<string, MethodInfo> METHOD_CACHE = new ConcurrentDictionary<string, MethodInfo>();

        private readonly ISerializer _serializer;
        public BaseService()
        {

            var serviceType = typeof(TInterFace);
            var serviceAttribute = (RpcServiceAttribute) serviceType.GetCustomAttribute(typeof(RpcServiceAttribute), false);
            this.ServiceId = serviceAttribute.ServiceId;
            this.GroupName = serviceAttribute.GroupName;

            this._serializer = Rpc.Internal.Environment.ServiceProvider.GetRequiredService<ISerializer>();
            //注册Group和Message
            Initialize(serviceType);
        }

        private void Initialize(Type serviceType)
        {
            var allMethods = serviceType.GetMethods(BindingFlags.Public);
            foreach (var method in allMethods)
            {
                var tt = method.GetCustomAttribute(typeof(RpcMethodAttribute), false);
                if (tt == null)
                {
                    continue;
                }
                var methodAttr = tt as RpcMethodAttribute;
                METHOD_CACHE.TryAdd($"{this.ServiceId}${methodAttr.MessageId}", method);
                //TODO:注册分组路由
            }
        }
        protected ushort ServiceId { get; }
        protected string GroupName { get; }
        public override string Id
        {
            get
            {
                return $"{this.ServiceId}%0";
            }
        }

        public override async Task<AmpMessage> ProcessAsync(AmpMessage req)
        {
            var resMsg = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            resMsg.Sequence = req.Sequence;
            string key = $"{req.ServiceId}${req.MessageId}";
            if (METHOD_CACHE.TryGetValue(key, out var m))
            {
                var pinfos = m.GetParameters();
                if(pinfos.Length <1 || pinfos.Length>2){
                    new RpcException("rpc method parameters count error,must be [1,2]");
                }
                object arg1 = this._serializer.Deserialize(req.Data,pinfos[0].ParameterType);
                bool withResponse = req.InvokeMessageType == InvokeMessageType.InvokeWithoutResponse;

                object retVal;
                if(pinfos.Length == 2){
                    if(pinfos[1].ParameterType == typeof(bool)){
                        retVal = m.Invoke(this,new object[]{arg1,withResponse});
                    }
                    else{
                        retVal = m.Invoke(this,new object[]{arg1,pinfos[1].HasDefaultValue?pinfos[1].DefaultValue:3000});
                    }
                }
                else{
                    retVal = m.Invoke(this,new object[]{arg1});
                }
                if(retVal.GetType() == typeof(Task)){
                    return resMsg;
                }

                Task<RpcResult> retTask = retVal as Task<RpcResult>;
                var result = await retTask;
                var resultType =result.GetType();
                object dataVal = null;
                if(resultType.BaseType == typeof(RpcResult)){ //泛型返回结果
                    var dataProp =  resultType.GetProperty("Data");
                    if(dataProp !=null){
                        dataVal = dataProp.GetValue(result);
                    }
                }
                resMsg.Code = result.Code;
                if(dataVal !=null){
                    resMsg.Data = this._serializer.Serialize(dataVal);
                }
            }
            else
            {
                resMsg.Code = RpcErrorCodes.CODE_SERVICE_NOT_FOUND;

            }
            return resMsg;
        }

        #region  IRpcService Method
        public override object Invoke(ushort messageId,params object[] args)
        {
            string key = $"{this.ServiceId}${messageId}";

            if (METHOD_CACHE.TryGetValue(key, out var m))
            {
                return m.Invoke(this,args);
            }
            throw new RpcException($"service method not found {key}");
        }

        #endregion
    }
}
