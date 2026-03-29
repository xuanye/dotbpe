// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.Impl;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DotBPE.Rpc.Tests.Client
{
    public class DefaultMessageSubscriberContainerTests
    {
        [Fact]
        public void GetMessageSubscribers_ShouldBeOk_FromInjectorContainer()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddSingleton<IMessageSubscriberContainer, DefaultMessageSubscriberContainer>();
            
            var subscriber1 = new Mock<IMessageSubscriber>();
            var subscriber2 = new Mock<IMessageSubscriber>();
            
            
            services.AddSingleton(subscriber1.Object);
            services.AddSingleton(subscriber2.Object);

            //act
            var provider =  services.BuildServiceProvider();
            
            var container = provider.GetRequiredService<IMessageSubscriberContainer>();

            var subscribers =  container.GetMessageSubscribers();
            
            //assert
            Assert.NotEmpty(subscribers);
            
            Assert.Equal(2,subscribers.Count);
        }

        [Fact]
        public void GetMessageSubscribers_ShouldBeBeOk_AfterSubscribeSingleOne()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddSingleton<IMessageSubscriberContainer, DefaultMessageSubscriberContainer>();
            
            var subscriber1 = new Mock<IMessageSubscriber>();
            var subscriber2 = new Mock<IMessageSubscriber>();

            services.AddSingleton(subscriber1.Object);
         

            //act && assert
            var provider =  services.BuildServiceProvider();
            
            var container = provider.GetRequiredService<IMessageSubscriberContainer>() as DefaultMessageSubscriberContainer;

            Assert.NotNull(container);
            
            var subscribersFirstTime =  container.GetMessageSubscribers();
            
            Assert.NotEmpty(subscribersFirstTime);
            Assert.Single(subscribersFirstTime);
            
            container.Subscribe(subscriber2.Object);
            
            var subscribersSecondTime =  container.GetMessageSubscribers();
         
            Assert.NotEmpty(subscribersSecondTime);
            
            Assert.Equal(2,subscribersSecondTime.Count);
        }
    }
}