// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Moq;
using Moq.AutoMock;
using Peach;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Rpc.Tests.Server
{
    public class DefaultMessageHandlerTests
    {

        [Fact]
        public async Task ReceiveResponseMessage_ShouldBe_DoNothing()
        {

            //arrange
            var autoMocker = new AutoMocker();
            var messageHandler = autoMocker.CreateInstance<DefaultMessageHandler>();
            var mockContext = new Mock<ISocketContext<AmpMessage>>();
            var message = new AmpMessage() { MessageType = RpcMessageType.Response };
            
            //act
            await messageHandler.ReceiveAsync(mockContext.Object, message);
        }

        [Fact]
        public async Task Receive_A_ValidMessage_HandlerFactory_Should_Find_Matched_Handler()
        {

            //arrange
            var autoMocker = new AutoMocker();
            var messageHandler = autoMocker.CreateInstance<DefaultMessageHandler>();
            var factoryMock = autoMocker.GetMock<IServiceActorHandlerFactory>();

            var mockContext = new Mock<ISocketContext<AmpMessage>>();
       

            var actorHandler = new Mock<IServiceActorHandler>();

            AmpMessage recievedMsg = null;
            actorHandler.Setup(x => x.HandleAsync(It.IsAny<ISocketContext<AmpMessage>>(), It.IsAny<AmpMessage>()))
                .Callback((ISocketContext<AmpMessage> context, AmpMessage msg) =>
                {
                    recievedMsg = msg;
                });
               

            factoryMock.Setup(x => x.GetInstance(It.IsAny<string>())).Returns(actorHandler.Object);
        
            var sendMsg = new AmpMessage() { MessageType = RpcMessageType.Request, ServiceId=100, MessageId=1};

            //act
            await messageHandler.ReceiveAsync(mockContext.Object, sendMsg);

            Assert.NotNull(recievedMsg);
            Assert.Equal(recievedMsg, sendMsg);
        }

        [Fact]
        public async Task Receive_A_Not_ValidMessage_HandlerFactory_Should_Find_Match_NotFoundHandler()
        {

            //arrange
            var autoMocker = new AutoMocker();
            var messageHandler = autoMocker.CreateInstance<DefaultMessageHandler>();
            var factoryMock = autoMocker.GetMock<IServiceActorHandlerFactory>();

            var mockContext = new Mock<ISocketContext<AmpMessage>>();

            AmpMessage recievedMsg =null;
            mockContext.Setup(x => x.SendAsync(It.IsAny<AmpMessage>())).Callback((AmpMessage msg) =>
            {
                recievedMsg = msg;
            });

            factoryMock.Setup(x => x.GetInstance(It.IsAny<string>())).Returns((IServiceActorHandler)null);

            var sendMsg = new AmpMessage() { MessageType = RpcMessageType.Request, ServiceId = 100, MessageId = 1 };

            //act
            await messageHandler.ReceiveAsync(mockContext.Object, sendMsg);

            Assert.NotNull(recievedMsg);

            Assert.Equal(sendMsg.ServiceId, recievedMsg.ServiceId);
            Assert.Equal(sendMsg.MessageId, recievedMsg.MessageId);
            Assert.Equal(RpcStatusCodes.CODE_SERVICE_NOT_FOUND, recievedMsg.Code);
        }
    }
}
