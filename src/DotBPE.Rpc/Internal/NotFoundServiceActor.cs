// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Peach;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Internal
{
    public class NotFoundServiceActor : IServiceActor<AmpMessage>
    {
        public static NotFoundServiceActor Instance = new NotFoundServiceActor();

        public string Id => "NotFoundServiceActor";
        public string GroupName => "default";

        public async Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {
            var response = new AmpMessage
            {
                MessageType = RpcMessageType.Response,
                Code = RpcStatusCodes.CODE_SERVICE_NOT_FOUND,
                ServiceId = message.ServiceId,
                MessageId = message.MessageId,
                Sequence = message.Sequence
            };
            await context.SendAsync(response);
        }

        public static bool IsNotFoundServiceActor(IServiceActor<AmpMessage> serviceActor)
        {
            return serviceActor.Id == Instance.Id;
        }

    }
}
