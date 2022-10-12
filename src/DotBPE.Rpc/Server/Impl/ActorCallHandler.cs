﻿// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Protocols;
using Peach;
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Server
{
    public class ActorCallHandler<TService, TRequest, TResponse>
        where TService : IServiceActor
        where TRequest : class
        where TResponse : class
    {
        private readonly IServiceActorLocator _serviceActor;
        private readonly MethodInvoker<TService, TRequest, TResponse> _invoker;
        private readonly ISerializer _serializer;

        private readonly Type _requestType;
        public ActorCallHandler(IServiceActorLocator serviceActor, MethodInvoker<TService, TRequest, TResponse> invoker, ISerializer serializer)
        {
            _serviceActor = serviceActor;
            _invoker = invoker;
            _serializer = serializer;
            _requestType = typeof(TRequest);
        }


        public async Task HandleCallAsync(ISocketContext<AmpMessage> context, AmpMessage reqMsg)
        {
            //TODO:这里可以添加服务拦截器的实现

            var actor = _serviceActor.LocateServiceActor(reqMsg.MethodIdentifier);

            var resMsg = AmpMessage.CreateResponseMessage(reqMsg);

            TRequest request = null;
            if (reqMsg.Data != null)
                request = (TRequest)_serializer.Deserialize(reqMsg.Data, _requestType);

            var result = await _invoker.InvokeAsync((TService)actor, request);

            resMsg.Code = result.Code;
            if (result.Data != null)
                resMsg.Data = _serializer.Serialize(result.Data);

            await context.SendAsync(resMsg);

        }


    }
}