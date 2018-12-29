using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBPE.Rpc.Server
{
    public class DefaultServiceActorLocator : IServiceActorLocator<AmpMessage>
    {
        static ConcurrentDictionary<string, IServiceActor<AmpMessage>> ACTOR_CACHE = new ConcurrentDictionary<string, IServiceActor<AmpMessage>>();
        static readonly HeartbeatActor HeartbeatActor = new HeartbeatActor();
        private readonly ILogger<DefaultServiceActorLocator> _logger;
        public DefaultServiceActorLocator(IServiceProvider serviceProvider)
        {
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = logFactory.CreateLogger<DefaultServiceActorLocator>();
            Initialize(serviceProvider);

           
        }
        
        private void Initialize(IServiceProvider serviceProvider)
        {
            var actorList = serviceProvider.GetServices<IServiceActor<AmpMessage>>();
            if (actorList != null && actorList.Count() >0)
            {  
                foreach (var actor in actorList)
                {
                    ACTOR_CACHE.AddOrUpdate(actor.Id, actor, (k, v) => actor);
                }               
            }
            else
            {
                _logger.LogInformation(" any service actor is not registered");
            }
        }
        /// <summary>
        /// Service Actor Locate
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IServiceActor<AmpMessage> LocateServiceActor(AmpMessage message)
        {
            if (message.IsHeartBeat)
            {
                //心跳消息
                return HeartbeatActor;
            }

            if (ACTOR_CACHE.TryGetValue(message.ServiceIdentifier,out var serviceActor))
            {
                return serviceActor;
            }

            if (ACTOR_CACHE.TryGetValue(message.MethodIdentifier, out var methodServiceActor))
            {
                return methodServiceActor;
            }

            return null;
        }
    }
}
