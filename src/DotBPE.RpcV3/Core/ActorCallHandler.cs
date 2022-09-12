// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Abstractions;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Protocols;
using Peach;
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Core
{
    public class ActorCallHandler<TService, TRequest, TResponse>
        where TService : class
        where TRequest : class
        where TResponse : class
    {
        private readonly MethodInvoker<TService, TRequest, TResponse> _invoker;
        private readonly ISerializer _serializer;

        private readonly Type _requestType;
        public ActorCallHandler(MethodInvoker<TService, TRequest, TResponse> invoker, ISerializer serializer)
        {
            _invoker = invoker;
            _serializer = serializer;
            _requestType = typeof(TRequest);
        }


        public async Task HandleCallAsync(IServiceActor serviceActor, ISocketContext<AmpMessage> context, AmpMessage reqMsg)
        {
            var resMsg = AmpMessage.CreateResponseMessage(reqMsg);

            TRequest? request = null;
            if (reqMsg.Data != null)
            {
                request = (TRequest)_serializer.Deserialize(reqMsg.Data, _requestType);
            }

            var result = await _invoker.InvokeAsync(serviceActor, request);

            resMsg.Code = result.Code;
            if (result.Data != null)
            {
                resMsg.Data = _serializer.Serialize(result.Data);
            }

            await context.SendAsync(resMsg);

        }


    }
}
