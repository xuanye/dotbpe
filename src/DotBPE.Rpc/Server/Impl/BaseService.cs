using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Environment = DotBPE.Rpc.Internal.Environment;

namespace DotBPE.Rpc.Server.Impl
{
    public class BaseService<TInterFace> : AbsServiceActor where TInterFace : class
    {
        static ConcurrentDictionary<string, MethodInfo> METHOD_CACHE = new ConcurrentDictionary<string, MethodInfo>();

        protected BaseService()
        {
            var serviceType = typeof(TInterFace);

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
        protected ushort ServiceId { get; }

        /// <summary>
        /// service group name
        /// </summary>
        protected string GroupName { get; }


        /// <summary>
        ///  Default serializer from IOC container
        /// </summary>
        protected ISerializer Serializer =>
            this._serializer ??
            (this._serializer = Environment.ServiceProvider.GetRequiredService<ISerializer>());

        public override string Id => $"{ServiceId}$0";

        #endregion

        private void Initialize(Type serviceType)
        {
            var allMethods = serviceType.GetMethods();
            foreach (var method in allMethods)
            {
                var tt = method.GetCustomAttribute(typeof(RpcMethodAttribute), false);
                if (tt == null)
                {
                    continue;
                }

                var methodAttr = tt as RpcMethodAttribute;
                METHOD_CACHE.TryAdd($"{ServiceId}${methodAttr.MessageId}", method);
            }
        }

        private RpcServiceAttribute GetServiceAttribute(Type serviceType)
        {
            var serviceAttribute = serviceType.GetCustomAttribute(
                typeof(RpcServiceAttribute), false) as RpcServiceAttribute;
            if (serviceAttribute == null)
            {
                throw new InvalidOperationException($"Miss RpcServiceAttribute at {serviceType}");
            }

            return serviceAttribute;

        }

        /// <summary>
        ///  Remote Call
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        protected override async Task<AmpMessage> ProcessAsync(AmpMessage req)
        {
            var resMsg = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            resMsg.Sequence = req.Sequence;
            string key = $"{req.ServiceId}${req.MessageId}";
            if (METHOD_CACHE.TryGetValue(key, out var m))
            {
                var parameterInfos = m.GetParameters();
                if (parameterInfos.Length < 1 || parameterInfos.Length > 2)
                {
                    new RpcException("rpc method parameters count error,must be [1,2]");
                }

                object arg1 = Serializer.Deserialize(req.Data, parameterInfos[0].ParameterType);
                bool withResponse = req.InvokeMessageType == InvokeMessageType.InvokeWithoutResponse;

                object retVal;
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
                    retVal = m.Invoke(this,newArgs);
                }
                else
                {
                    retVal = m.Invoke(this, new[] {arg1});
                }

                var retValType = retVal.GetType();
                if (retValType == typeof(Task))
                {
                    return resMsg;
                }

                var tType = retValType.GenericTypeArguments[0];
                if (tType == typeof(RpcResult))
                {
                    Task<RpcResult> retTask = retVal as Task<RpcResult>;
                    var result = await retTask;
                    resMsg.Code = result.Code;
                }
                else if (tType.IsGenericType)
                {
                    Task retTask = retVal as Task;
                    await retTask.AnyContext();

                    var resultProp = retValType.GetProperty("Result");
                    if (resultProp == null)
                    {
                        resMsg.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                        return resMsg;
                    }

                    object result = resultProp.GetValue(retVal);

                    object dataVal = null;
                    var dataProp = tType.GetProperty("Data");
                    if (dataProp != null)
                    {
                        dataVal = dataProp.GetValue(result);
                    }

                    if (dataVal != null)
                    {
                        resMsg.Data = Serializer.Serialize(dataVal);
                    }
                }
                else
                {
                    resMsg.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    //LOG Error;
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
