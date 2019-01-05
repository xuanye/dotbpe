using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotBPE.Rpc.Exceptions;

namespace DotBPE.Rpc.Server
{
    public class DefaultServiceActorLocator : IServiceActorLocator<AmpMessage>
    {
        private readonly ConcurrentDictionary<string, IServiceActor<AmpMessage>> ACTOR_CACHE = new ConcurrentDictionary<string, IServiceActor<AmpMessage>>();
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
            if (actorList != null && actorList.Any())
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

            string[] path = servicePath.Split('.');

            string serviceId = string.Empty;
            string messageId =  string.Empty;

            if (path.Length == 2)
            {
                serviceId = path[0];
                messageId = path[1];
            }
            else if (path.Length == 3)
            {
                serviceId = path[1];
                messageId = path[2];
            }
            else
            {
                this._logger.LogError("服务路径错误:{servicePath}",servicePath);
                throw  new RpcException($"服务路径错误:{servicePath}");
            }


            string serviceKey = $"{serviceId}.0";
            string messageKey = $"{serviceId}.{messageId}";

            var actor = GetFromCache(messageKey);
            if (actor != null)
            {
                return actor;
            }


            actor = GetFromCache(serviceKey);

            if (actor != null)
            {
                return actor;
            }

            return GetNotFoundActor();
        }

        protected virtual IServiceActor<AmpMessage> GetFromCache(string cacheKey)
        {
            if (ACTOR_CACHE.TryGetValue(cacheKey,out var serviceActor))
            {
                return serviceActor;
            }

            return null;

        }

        protected virtual IServiceActor<AmpMessage> GetNotFoundActor()
        {
            return NotFoundServiceActor.Default;
        }
    }
}
