using DotBPE.Baseline.Extensions;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Internal
{
    public static class InternalHelper
    {
        public static string FormatServiceIdentity(string serviceGroupName, int serviceId, ushort messageId)
        {
            return $"{serviceGroupName}.{serviceId}.{messageId}";
        }

        public static string FormatServicePath(int serviceId, ushort messageId)
        {
            return $"{serviceId}.{messageId}";
        }

        public static async Task<RpcResult<object>> DrillDownResponseObj(object retVal)
        {

            var result = new RpcResult<object>();
            var retValType = retVal.GetType();
            if (retValType == typeof(Task))
            {
                return result;
            }


            var tType = retValType.GenericTypeArguments[0];
            if (tType == typeof(RpcResult))
            {
                var retTask = retVal as Task<RpcResult>;
                var tmp = await retTask;
                result.Code = tmp.Code;
                return result;
            }

            if (tType.IsGenericType)
            {
                Task retTask = retVal as Task;
                await retTask.AnyContext();

                var resultProp = retValType.GetProperty("Result");
                if (resultProp == null)
                {
                    result.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    return result;
                }

                object realVal = resultProp.GetValue(retVal);

                object dataVal = null;
                var dataProp = tType.GetProperty("Data");
                if (dataProp != null)
                {
                    dataVal = dataProp.GetValue(realVal);
                }

                var codeProp = tType.GetProperty("Code");
                if (codeProp == null)
                {
                    result.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    return result;
                }

                result.Code = (int)codeProp.GetValue(realVal);

                if (dataVal != null)
                {
                    result.Data = dataVal;
                }
                return result;
            }
            return null;
        }
    }
}
