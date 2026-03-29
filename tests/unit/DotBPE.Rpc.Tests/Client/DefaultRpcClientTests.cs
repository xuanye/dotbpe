// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocols;
using Moq;
using Moq.AutoMock;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Rpc.Tests.Client
{
    public class DefaultRpcClientTests
    {

        [Fact]
        public async Task SendAsync_MessageHandlerRecieveErrorRespons_RouterPointNotFound()
        {
            //arrange
            var autoMocker = new AutoMocker();

            var client = autoMocker.CreateInstance<DefaultRpcClient>();

            var routerMocker = autoMocker.GetMock<IServiceRouter>();

            routerMocker.Setup(x => x.FindRouterPoint(It.IsAny<string>()))
                .Returns(Task.FromResult<IRouterPoint>(null));

            var handlerMocker = autoMocker.GetMock<IClientMessageHandler>();

            AmpMessage recieveMsg = null;
            handlerMocker.Setup(x => x.RaiseReceive(It.IsAny<AmpMessage>()))
                .Callback((AmpMessage msg) =>
                {
                    recieveMsg = msg;
                });

            int serviceId = 100;
            ushort messageId = 1;
            var msg = AmpMessage.CreateRequestMessage(serviceId, messageId, CodecType.JSON);

            //act
            await client.SendAsync(msg);

            //assert

            Assert.NotNull(recieveMsg);
            Assert.Equal(serviceId, recieveMsg.ServiceId);
            Assert.Equal(messageId, recieveMsg.MessageId);
            Assert.Equal(msg.Sequence, recieveMsg.Sequence);
            Assert.Equal(RpcStatusCodes.CODE_INTERNAL_ERROR, recieveMsg.Code);

        }


        [Fact]
        public async Task SendAsync_MessageHandlerRecieveErrorRespons_RouterPointIsLocal()
        {
            //arrange
            var autoMocker = new AutoMocker();

            var client = autoMocker.CreateInstance<DefaultRpcClient>();

            var routerMocker = autoMocker.GetMock<IServiceRouter>();

            var localPoint = new RouterPoint() { RoutePointType = RoutePointType.Local };
            routerMocker.Setup(x => x.FindRouterPoint(It.IsAny<string>()))
                .Returns(Task.FromResult<IRouterPoint>(localPoint));

            var handlerMocker = autoMocker.GetMock<IClientMessageHandler>();

            AmpMessage recieveMsg = null;
            handlerMocker.Setup(x => x.RaiseReceive(It.IsAny<AmpMessage>()))
                .Callback((AmpMessage msg) =>
                {
                    recieveMsg = msg;
                });

            int serviceId = 100;
            ushort messageId = 1;
            var msg = AmpMessage.CreateRequestMessage(serviceId, messageId, CodecType.JSON);

            //act
            await client.SendAsync(msg);

            //assert

            Assert.NotNull(recieveMsg);
            Assert.Equal(serviceId, recieveMsg.ServiceId);
            Assert.Equal(messageId, recieveMsg.MessageId);
            Assert.Equal(msg.Sequence, recieveMsg.Sequence);
            Assert.Equal(RpcStatusCodes.CODE_INTERNAL_ERROR, recieveMsg.Code);

        }


        [Fact]
        public async Task SendAsync_TransportRecieveMessage_RouterPointIsRemote()
        {
            //arrange
            var autoMocker = new AutoMocker();

            var client = autoMocker.CreateInstance<DefaultRpcClient>();

            var routerMocker = autoMocker.GetMock<IServiceRouter>();

            var remotePoint = new RouterPoint() { RoutePointType = RoutePointType.Remote };
            routerMocker.Setup(x => x.FindRouterPoint(It.IsAny<string>()))
                .Returns(Task.FromResult<IRouterPoint>(remotePoint));


            AmpMessage recieveMsg = null;

            var transportMocker = new Mock<ITransport>();
            transportMocker.Setup(x => x.SendAsync(It.IsAny<AmpMessage>()))
                  .Callback((AmpMessage msg) =>
                  {
                      recieveMsg = msg;
                  });

            var transportFactoryMocker = autoMocker.GetMock<ITransportFactory>();


            transportFactoryMocker.Setup(x => x.CreateTransport(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(transportMocker.Object));

            int serviceId = 100;
            ushort messageId = 1;
            var msg = AmpMessage.CreateRequestMessage(serviceId, messageId, CodecType.JSON);

            //act
            await client.SendAsync(msg);

            //assert

            Assert.NotNull(recieveMsg);
            Assert.Equal(serviceId, recieveMsg.ServiceId);
            Assert.Equal(messageId, recieveMsg.MessageId);
            Assert.Equal(msg.Sequence, recieveMsg.Sequence);
            Assert.Equal(RpcStatusCodes.CODE_SUCCESS, recieveMsg.Code);

        }
    }
}
