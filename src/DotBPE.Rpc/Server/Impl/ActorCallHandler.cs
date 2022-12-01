// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.AuditLog;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Protocols;
using Microsoft.Extensions.Logging;
using Peach;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class ActorCallHandler<TService, TRequest, TResponse>
        where TService : class
        where TRequest : class
        where TResponse : class
    {
        private readonly IServiceActorLocator _serviceActor;
        private readonly MethodInvoker<TService, TRequest, TResponse> _invoker;
        private readonly ISerializer _serializer;
        private readonly IAuditLoggerFactory _auditLoggerFactory;
        private readonly Type _requestType;
        private readonly ILogger _logger;
        public ActorCallHandler(IServiceActorLocator serviceActor
            , MethodInvoker<TService, TRequest, TResponse> invoker
            , ISerializer serializer
            , ILoggerFactory loggerFactory
            , IAuditLoggerFactory auditLoggerFactory = null
            )
        {
            _logger = loggerFactory.CreateLogger("ActorCallHandler");
            _serviceActor = serviceActor;
            _invoker = invoker;
            _serializer = serializer;
            _auditLoggerFactory = auditLoggerFactory;
            _requestType = typeof(TRequest);
        }


        public async Task HandleCallAsync(ISocketContext<AmpMessage> context, AmpMessage reqMsg)
        {
            var actor = _serviceActor.LocateServiceActor(reqMsg.MethodIdentifier);
            var resMsg = AmpMessage.CreateResponseMessage(reqMsg);
            var sw = new Stopwatch();
            sw.Start();
            var result = default(RpcResult<TResponse>);
            TRequest request = null;
            try
            {

                if (reqMsg.Data != null)
                    request = (TRequest)_serializer.Deserialize(reqMsg.Data, _requestType);

                result = await _invoker.InvokeAsync((TService)actor, request);
                resMsg.Code = result.Code;
                if (result.Data != null)
                    resMsg.Data = _serializer.Serialize(result.Data);
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, $"call service method error:{rpcEx.Message}");
                resMsg.Code = rpcEx.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"call service method error:{ex.Message}");

                resMsg.Code = RpcStatusCodes.CODE_INTERNAL_ERROR;
            }

            try
            {
                await context.SendAsync(resMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"send repsonse to client error:{ex.Message}");
                resMsg.Code = RpcStatusCodes.CODE_TIMEOUT;
            }
            sw.Stop();

            if (_auditLoggerFactory != null)
            {
                var logger = _auditLoggerFactory.GetLogger(AuditLogType.Service);
                if (logger != null)
                    await logger.Log(reqMsg.FriendlyServiceName, request, result.Data, result.Code, sw.ElapsedMilliseconds, new RpcContext(context.LocalEndPoint, context.RemoteEndPoint));
            }

        }


    }
}
