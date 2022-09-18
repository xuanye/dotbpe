// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DotBPE.Rpc.Server
{
    public class DefaultServiceActorHandlerFactory : IServiceActorHandlerFactory
    {
        private static readonly ConcurrentDictionary<string, ActorInvokerModel> _invokerCache = new ConcurrentDictionary<string, ActorInvokerModel>();
        private static readonly ConcurrentDictionary<string, IServiceActorHandler> _handlerCache = new ConcurrentDictionary<string, IServiceActorHandler>();
        private readonly ILogger<DefaultServiceActorHandlerFactory> _logger;
        private readonly IServiceActorLocator _actorLocator;

        public DefaultServiceActorHandlerFactory(
            ILogger<DefaultServiceActorHandlerFactory> logger,
            IServiceActorLocator actorLocator
            )
        {
            _logger = logger;
            _actorLocator = actorLocator;
        }



        public IServiceActorHandler GetInstance(string methodIdentifier)
        {
            if (_handlerCache.TryGetValue(methodIdentifier, out var handler))
                return handler;

            if (!_invokerCache.TryGetValue(methodIdentifier, out var invoker))
                return NotFoundServiceActorHandler.Instance;
            var serviceActor = _actorLocator.LocateServiceActor(methodIdentifier);

            handler = new ServiceActorHandler(serviceActor, invoker);
            _handlerCache.TryAdd(methodIdentifier, handler);
            return handler;
        }

        public void RegisterActorInvokerHandler(ActorInvokerModel actorModel)
        {
            if (!_invokerCache.TryAdd(actorModel.Method.FullName, actorModel))
                _logger.LogWarning("{MethodFullName} has registration conflicts", actorModel.Method.FullName);
            else
            {
                _logger.LogInformation("{MethodFullName} has been registered", actorModel.Method.FullName);
            }
        }
    }


}
