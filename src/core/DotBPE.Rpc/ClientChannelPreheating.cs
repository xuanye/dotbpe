using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Options;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Options;

namespace DotBPE.Rpc {
    public class ClientChannelPreheating<TMessage> : IPreheating where TMessage : InvokeMessage {
        private readonly IOptions<RemoteServicesOption> _options;
        private readonly ITransportFactory<TMessage> _factory;

        public ClientChannelPreheating (IOptions<RemoteServicesOption> options, ITransportFactory<TMessage> factory) {
            this._factory = factory;
            this._options = options;
        }

        public Task StartAsync () {
            //从配置中获取remoteServices的配置，读取address的信息
            List<string> remoteAddress = new List<string> ();
            if (this._options != null && this._options.Value != null) {
                var routeOptions = this._options.Value;
                foreach (var option in routeOptions) {
                    string[] arrAdd = option.RemoteAddress.Split (',');
                    foreach (string address in arrAdd) {
                        if (remoteAddress.IndexOf (address) < 0) {
                            remoteAddress.Add (address);
                        }
                    }
                }
            }
            if (remoteAddress.Count > 0) {
                return Task.Factory.StartNew (() => {
                    for (var i = 0; i < remoteAddress.Count; i++) {
                        EndPoint point = ParseUtils.ParseEndPointFromString (remoteAddress[i]);
                        this._factory.CreateTransport (point);
                    }
                });
            }
            return TaskUtils.CompletedTask;
        }
    }
}