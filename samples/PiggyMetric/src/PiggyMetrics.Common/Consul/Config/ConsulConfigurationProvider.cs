using System;
using System.Net;
using Consul;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PiggyMetrics.Common.Consul;
using DotBPE.Rpc.Logging;

namespace PiggyMetrics.Common
{
    public class ConsulConfigurationProvider:ConfigurationProvider
    {
        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<ConsulConfigurationProvider>();
        private ConsulConfigurationOptions _Options;
        private readonly ConsulClient _Client;

        private ulong _LastIndex =0 ;

        private bool _StopCheck = false;

        private QueryOptions _queryOptions ;
        public ConsulConfigurationProvider(ConsulConfigurationOptions options,ConsulClient client){
            this._Options = options;
            this._Client = client;
            _queryOptions = new QueryOptions(){
                WaitIndex = 0,
                WaitTime = TimeSpan.FromMinutes(5)
            };
            Initialize();
        }

        private void Initialize()
        {
            if(this._Options.ReloadOnChange){
               //定时刷新并判断是否更新了
               ThreadPool.QueueUserWorkItem( CheckChanged );
            }
        }

        private void CheckChanged(object state){
            while(!_StopCheck){ //
                //Thread.Sleep(this._Options.CheckInterval);
                LoadData(true,true).Wait();
                Logger.Debug("check data completed");
            }
        }
        public override void Load()
        {
            LoadData(false, false).Wait();
        }

        private async Task LoadData(bool reloading =false,bool check =false){

            try{

                if(reloading){
                    _queryOptions.WaitIndex = _LastIndex+1;
                }
                var result = await this._Client.KV.Get(this._Options.Key,_queryOptions) ;

                if( result.StatusCode == HttpStatusCode.OK){
                    if(this._LastIndex != result.LastIndex){
                        this._LastIndex = result.LastIndex;
                        ParseFrom(result.Response);
                    }
                }
                else if(result.StatusCode == HttpStatusCode.NotFound && !reloading && !check){
                    throw new Exception($"The configuration for key {this._Options.Key} was not found and is not optional.");
                }
                else{
                    Console.WriteLine("load data error,{0}",result.StatusCode);
                    if(!check)
                        throw new Exception($"request consul error return code {result.StatusCode}.");
                }


           }
           catch(Exception ex){
                HandlerLoadException(ex);
           }
        }

        private void HandlerLoadException(Exception ex)
        {
            var args = new ExceptionCaughtEventArgs();

            this._Options.ExceptionCaught?.Invoke(ex,new ExceptionCaughtEventArgs());

            if(!args.Ingore){
                throw ex;
            }
        }

        private void ParseFrom(KVPair kv)
        {

            if (kv.Value == null || kv.Value.Length == 0)
            {
                return;
            }
            var parser = new JsonConfigurationParser();
            using (var stream = new MemoryStream(kv.Value))
            {
                Data = parser.Parse(stream);
            }
            if(this._LastIndex>0){
                Logger.Debug("raise base onreload");
                base.OnReload();
            }
            //Console.WriteLine(JsonConvert.SerializeObject(Data));
        }


    }
}
