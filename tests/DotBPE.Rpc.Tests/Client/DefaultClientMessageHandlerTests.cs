// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.Impl;
using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DotBPE.Rpc.Tests.Client
{
    public class DefaultClientMessageHandlerTests
    {
        [Fact]
        public void RaiseReceive_SubscribersReceiveSameMessage()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddSingleton<IClientMessageHandler, DefaultClientMessageHandler>();
            services.AddSingleton<IMessageSubscriberContainer, DefaultMessageSubscriberContainer>();

            var msg = AmpMessage.CreateResponseMessage("100|1");


            var subscriber1 = new Mock<IMessageSubscriber>();
            var subscriber2 = new Mock<IMessageSubscriber>();

            AmpMessage receivedMessage1 = null;
            AmpMessage receivedMessage2 = null;
            subscriber1.Setup(x => x.Handle(It.IsAny<AmpMessage>())).Callback((AmpMessage m1) =>
            {
                receivedMessage1 = m1;
            });
            subscriber2.Setup(x => x.Handle(It.IsAny<AmpMessage>())).Callback((AmpMessage m2) =>
            {
                receivedMessage2 = m2;
            });

            services.AddSingleton(subscriber1.Object);
            services.AddSingleton(subscriber2.Object);

            //act
            var provider = services.BuildServiceProvider();
            var messageHandler = provider.GetRequiredService<IClientMessageHandler>();
            messageHandler.RaiseReceive(msg);

            //assert
            Assert.NotNull(receivedMessage1);
            Assert.NotNull(receivedMessage2);
            Assert.Equal(msg, receivedMessage1);
            Assert.Equal(msg, receivedMessage2);
        }
    }
}
