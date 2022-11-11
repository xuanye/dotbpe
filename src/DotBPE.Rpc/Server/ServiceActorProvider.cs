// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.AuditLog;
using Microsoft.Extensions.Logging;
using System;

namespace DotBPE.Rpc.Server
{
    public class ServiceActorProvider<TService> : IServiceActorProvider
        where TService : class
    {
        private readonly IServiceActorLocator _actorLocator;
        private readonly ISerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IAuditLoggerFactory _auditLoggerFactory;

        public ServiceActorProvider(IServiceActorLocator actorLocator
            , ISerializer serializer
            , ILoggerFactory loggerFactory
            , IAuditLoggerFactory auditLoggerFactory = null)
        {
            _actorLocator = actorLocator;
            _serializer = serializer;
            _loggerFactory = loggerFactory;
            _auditLoggerFactory = auditLoggerFactory;
        }


        public void OnServiceActorDiscovery(ServiceActorProviderContext context)
        {
            try
            {

                var binder = new ServiceActorBinder<TService>(context, _actorLocator, _serializer, _loggerFactory, _auditLoggerFactory);
                binder.Bind();

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding RPC service '{typeof(TService).Name}'.", ex);
            }
        }


    }
}
