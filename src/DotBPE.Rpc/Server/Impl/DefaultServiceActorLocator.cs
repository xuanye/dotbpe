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
            this._logger = logFactory.CreateLogger<DefaultServiceActorLocator>();
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
                this._logger.LogWarning("no service actor was registered");
            }
        }
        /// <summary>
        /// Service Actor Locate
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IServiceActor<AmpMessage> LocateServiceActor(string servicePath)
        {
            if (HeartbeatActor.Id.Equals(servicePath))
            {
                //心跳消息
                return HeartbeatActor;
            }
            string[] path = servicePath.Split('$');
            string serviceKey = $"{path[0]}$0";

            if (ACTOR_CACHE.TryGetValue(serviceKey, out var serviceActor))
            {
                return serviceActor;
            }

            if (ACTOR_CACHE.TryGetValue(servicePath,out serviceActor))
            {
                return serviceActor;
            }
            return NotFoundServiceActor.Default;
        }
    }
}
