using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc.Protocol;

namespace DotBPE.Rpc.Server.Impl
{
    public class BaseService<TInterFace> : AbsServiceActor where TInterFace:class,IRpcService
    {
        public BaseService()
        {
            var serviceType = typeof(TInterFace);
            var serviceAttribute = (RpcServiceAttribute)serviceType.GetCustomAttributes(typeof(RpcServiceAttribute), false).FirstOrDefault();
            this.ServiceId = serviceAttribute.ServiceId;


            //注册Group和Message
        }

        protected ushort ServiceId { get; }
        public override string Id {
            get {
                return  $"{this.ServiceId}%0";
            }
        }

        public override Task<AmpMessage> ProcessAsync(AmpMessage req)
        {
            throw new NotImplementedException();
        }
    }
}
