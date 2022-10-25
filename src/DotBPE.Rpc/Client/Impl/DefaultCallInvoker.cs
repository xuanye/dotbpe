// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.AuditLog;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public class DefaultCallInvoker : ICallInvoker, IMessageSubscriber
    {
        private readonly IRpcClient _rpcClient;
        private readonly ISerializer _serializer;
        private readonly ILogger<DefaultCallInvoker> _logger;
        private readonly IAuditLoggerFactory _auditLoggerFactory;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>> _resultDictionary = new ConcurrentDictionary<string, TaskCompletionSource<AmpMessage>>();

        private static int _invokerSeq;
        public DefaultCallInvoker(IRpcClient rpcClient
            , ISerializer serializer
            , ILogger<DefaultCallInvoker> logger
            , IAuditLoggerFactory auditLoggerFactory = null
            )
        {
            _rpcClient = rpcClient;
            _serializer = serializer;
            _logger = logger;
            _auditLoggerFactory = auditLoggerFactory;
        }

        public async Task<RpcResult<TResponse>> InvokerAsync<TRequest, TResponse>(IMethod method, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            var result = new RpcResult<TResponse>();
            var reqMessage = AmpMessage.CreateRequestMessage(method.ServiceId, method.MethodId, (CodecType)_serializer.CodecType);
            reqMessage.FriendlyServiceName = method.FullName;
            reqMessage.ServiceGroupName = method.GroupName;
            reqMessage.Data = _serializer.Serialize(request);

            var sw = new Stopwatch();
            sw.Start();
            var rsp = await AsyncCallInner(reqMessage, method.DefaultTimeout > 0 ? method.DefaultTimeout : 3000);
            if (rsp != null)
            {
                result.Code = rsp.Code;
                if (rsp.Data != null && rsp.Data.Length > 0)
                {
                    result.Data = _serializer.Deserialize<TResponse>(rsp.Data);
                }
            }
            else
            {
                _logger.LogError("Call {0} , return null", method.FullName);
                result.Code = RpcStatusCodes.CODE_INTERNAL_ERROR;
            }
            sw.Stop();
            //audit logger
            if (_auditLoggerFactory != null)
            {
                var logger = _auditLoggerFactory.GetLogger(AuditLogType.Client);
                if (logger != null)
                    await logger.Log(method.FullName, request, result.Data, result.Code, sw.ElapsedMilliseconds, LocalRpcContext.Instance);
            }
            return result;
        }




        public void Handle(AmpMessage message)
        {

            if (message.ServiceId == 0 && message.MessageId == 0)
            {
                _logger.LogTrace("received heart beat");
                return;
            }

            TaskCompletionSource<AmpMessage> task;
            if (message.Code != 0)
            {
                _logger.LogDebug("server response error msg ,type={0}", message.MessageType);
                if (_resultDictionary.TryRemove(message.Id, out task))
                {
                    task.TrySetResult(message);
                    _logger.LogDebug("message {0},set result success,message.code ={1}", message.Id, message.Code);
                }
                else
                {
                    _logger.LogError($"server response error msg ,id={message.Id},code={message.Code},");
                }
            }
            else
            {
                _logger.LogDebug($"receive message, id:{message.Id}");
                if (_resultDictionary.TryRemove(message.Id, out task))
                {
                    task.TrySetResult(message);
                    _logger.LogDebug("message {0},set result success,message.code ={1}", message.Id, message.Code);
                }
            }
        }

        private async Task<AmpMessage> AsyncCallInner(AmpMessage request, int timeout)
        {

            AutoSetSequence(request);
            _logger.LogDebug("new request id={0},type={1}", request.Id, request.MessageType);

            var cts = new CancellationTokenSource(timeout);
            //timeout callback
            using (cts.Token.Register(() => RaiseTimeoutCallBack(request.Id), false))
            {
                //register callback
                var callbackTask = RegisterResultCallbackAsync(request.Id);

                //async call
                await SendAsync(request);

                //get return message
                var rsp = await callbackTask;

                return rsp;
            }
        }
        private Task<AmpMessage> RegisterResultCallbackAsync(string id)
        {
            var tcs = new TaskCompletionSource<AmpMessage>();

            if (!_resultDictionary.TryAdd(id, tcs))
            {
                _logger.LogWarning("add result callback fail,the task is already exists,messageId:{MessageId}", id);
            }
            return tcs.Task;
        }

        private async Task SendAsync(AmpMessage request)
        {
            try
            {
                await _rpcClient.SendAsync(request);
            }
            catch (ClosedChannelException closedEx)
            {
                _logger.LogError(closedEx, "send message error,channel closed,{messageId}", request.Id);
                throw new RpcCommunicationException($"send message error,channel closed,{request.Id}", closedEx);
            }
            catch (Exception exception)
            {
                RaiseErrorCallBack(request.Id);
                _logger.LogError(exception, "error occ:");
            }

        }
        private void RaiseTimeoutCallBack(string id)
        {
            if (_resultDictionary.TryRemove(id, out var task))
            {
                var message = AmpMessage.CreateResponseMessage(id);
                message.Code = RpcStatusCodes.CODE_TIMEOUT;
                if (!task.TrySetResult(message))
                {
                    _logger.LogWarning("set timeout result fail,maybe task is completed,message {0}", id);
                }
                _logger.LogWarning("message {0}, timeout", id);
            }
        }


        private void RaiseErrorCallBack(string id)
        {
            if (_resultDictionary.TryRemove(id, out var task))
            {
                var message = AmpMessage.CreateResponseMessage(id);
                message.Code = RpcStatusCodes.CODE_INTERNAL_ERROR;
                if (!task.TrySetResult(message))
                {
                    _logger.LogWarning("set error result fail,maybe task is completed");
                }
                _logger.LogWarning("message {0}, error", id);
            }
            else
            {
                _logger.LogWarning("raise error callback failed,the task is not found, message {0}", id);
            }
        }


        private void AutoSetSequence(AmpMessage request)
        {
            int id = Interlocked.Increment(ref _invokerSeq);
            request.Sequence = id;
        }


    }
}
