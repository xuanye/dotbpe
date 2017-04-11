using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Protocol.Amp
{
    public abstract class LocalServiceActor : IServiceActor<AmpMessage>
    {
        public LocalServiceActor()
        {

        }
        /// <summary>
        /// 当前服务的标识
        /// </summary>
        public virtual string Id {
            get
            {
                return "AmpLocalServiceActor";
            }
        }
       

        public Task Receive(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            //TODO:本地服务的话
            throw new NotImplementedException();
        }
    }
}
