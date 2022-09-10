using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using DotBPE.Rpc.Codec;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Peach;
using Environment = DotBPE.Rpc.Internal.Environment;

namespace DotBPE.Rpc.Server
{
    public class BaseService<TService> : AbsServiceActor where TService : class
    {
        protected BaseService()
        {
            var serviceType = typeof(TService);

            var serviceAttribute = GetServiceAttribute(serviceType);

            ServiceId = serviceAttribute.ServiceId;
            GroupName = serviceAttribute.GroupName;
            //注册Group和Message
            Initialize(serviceType);
        }

        #region Properties

        private ISerializer _serializer;

        /// <summary>
        /// service id
        /// </summary>
        protected int ServiceId { get; }




        /// <summary>
        ///  Default serializer from IOC container
        /// </summary>
        private ISerializer Serializer =>
            this._serializer ??
            (this._serializer = Environment.ServiceProvider.GetRequiredService<ISerializer>());


        private IRequestAuditLoggerFactory _auditLoggerFactory;

        private IRequestAuditLoggerFactory AuditLoggerFactory =>
            this._auditLoggerFactory ??
            (this._auditLoggerFactory = Environment.ServiceProvider.GetRequiredService<IRequestAuditLoggerFactory>());


        public override string Id => $"{ServiceId}.0";
        /// <summary>
        /// service group name
        /// </summary>
        public override string GroupName { get; } = "default";

        #endregion

        private void Initialize(Type serviceType)
        {
            foreach (var method in serviceType.GetMethods())
            {
                var methodAttr = method.GetCustomAttribute<RpcMethodAttribute>( false);
                if (methodAttr == null)
                {
                    continue;
                }
                MethodDescriptorHelper.TryAdd($"{ServiceId}${methodAttr.MessageId}", method);
            }
        }

        private static RpcServiceAttribute GetServiceAttribute(Type serviceType)
        {
            var serviceAttribute = serviceType.GetCustomAttribute<RpcServiceAttribute>(false);
            if (serviceAttribute == null)
            {
                throw new InvalidOperationException($"Miss RpcServiceAttribute at {serviceType}");
            }

            return serviceAttribute;

        }

        private Task<RpcResult<object>> InvokeInner(MethodInfo method, ISocketContext<AmpMessage> context, params object[] args)
        {
            Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");

            var serviceNameArr = method.DeclaringType.Name.Split('`');
            var methodFullName = $"{serviceNameArr[0]}.{method.Name}";

            using (var logger = this.AuditLoggerFactory.GetLogger(methodFullName))
            {
                logger.SetParameter(args[0]);
                logger.SetContext(new RpcContext { LocalAddress = context.LocalEndPoint,RemoteAddress = context.RemoteEndPoint });
                object retVal = method.Invoke(this, args);
                var result = InternalHelper.DrillDownResponseObj(retVal);
                logger.SetReturnValue(result);
                return result;
            }
        }

        /// <summary>
        ///  Remote Call
        /// </summary>
        /// <param name="context"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        protected override async Task<AmpMessage> ProcessAsync(ISocketContext<AmpMessage> context, AmpMessage req)
        {
            var resMsg = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            resMsg.Sequence = req.Sequence;
            resMsg.CodecType = (CodecType)Enum.ToObject(typeof(CodecType), Serializer.CodecType);

            var key = $"{req.ServiceId}${req.MessageId}";
            if (MethodDescriptorHelper.TryGetValue(key, out var m))
            {
                var parameterInfos = m.GetParameters();
                if (parameterInfos.Length < 1 || parameterInfos.Length > 2)
                {
                    throw new RpcException("rpc method parameters count error,must be [1,2]");
                }

                var arg1 = Serializer.Deserialize(req.Data, parameterInfos[0].ParameterType);
                var withResponse = req.InvokeMessageType == InvokeMessageType.InvokeWithoutResponse;

                RpcResult<object> retVal;
                if (parameterInfos.Length == 2)
                {
                    var newArgs = new object[2];
                    newArgs[0] = arg1;

                    if (parameterInfos[1].ParameterType == typeof(bool))
                    {
                        newArgs[1] = withResponse;
                    }
                    else
                    {
                        newArgs[1] = parameterInfos[1].HasDefaultValue ? parameterInfos[1].DefaultValue : 3000;
                    }

                    retVal = await InvokeInner(m, context, newArgs);
                }
                else
                {
                    retVal = await InvokeInner(m, context, arg1);
                }

                if(retVal == null)
                {
                    resMsg.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    return resMsg;
                }

                resMsg.Code = retVal.Code;
                if(retVal.Data != null)
                {
                    resMsg.Data = Serializer.Serialize(retVal.Data);
                }
            }
            else
            {
                resMsg.Code = RpcErrorCodes.CODE_SERVICE_NOT_FOUND;
            }

            return resMsg;
        }

    }
}
