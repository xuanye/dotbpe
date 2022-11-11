// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotBPE.Rpc.Server
{
    /// <summary>
    /// 根据配置信息获取本地或者远程的
    /// </summary>
    public class DefaultServiceActorLocator : IServiceActorLocator
    {
        private readonly ILogger<DefaultServiceActorLocator> _logger;

        private readonly ConcurrentDictionary<string, IServiceActor> _actorCaches =
          new ConcurrentDictionary<string, IServiceActor>();

        public DefaultServiceActorLocator(IEnumerable<IServiceActor> serviceActors, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DefaultServiceActorLocator>();
            Initialize(serviceActors);
        }

        private void Initialize(IEnumerable<IServiceActor> serviceActors)
        {
            if (serviceActors.Any())
            {
                foreach (var actor in serviceActors)
                {
                    _actorCaches.AddOrUpdate(actor.Id, actor, (k, v) => actor);
                }
            }
            else
            {
                _logger.LogWarning("no service actor was registered");
            }
        }

        public IServiceActor LocateServiceActor(string actorId)
        {

            var parts = actorId.Split('.');

            string serviceId;
            string methodId;

            switch (parts.Length)
            {
                case 2:
                    serviceId = parts[0];
                    methodId = parts[1];
                    break;
                case 3:
                    serviceId = parts[1];
                    methodId = parts[2];
                    break;
                default:
                    _logger.LogError("ServiceActor not found:{ActorId}", actorId);
                    throw new RpcException(RpcStatusCodes.CODE_SERVICE_NOT_FOUND, $"ServiceActor not found:{actorId}");
            }

            var serviceKey = $"{serviceId}.0";
            var messageKey = $"{serviceId}.{methodId}";


            var actor = GetFromCache(messageKey);
            if (actor != null)
                return actor;

            actor = GetFromCache(serviceKey);

            return actor ?? GetNotFoundActor();
        }
        protected virtual IServiceActor GetFromCache(string cacheKey)
        {
            return _actorCaches.TryGetValue(cacheKey, out var serviceActor)
                    ? serviceActor
                    : null;
        }
        protected virtual IServiceActor GetNotFoundActor()
        {
            return NotFoundServiceActor.Instance;
        }
    }
}
