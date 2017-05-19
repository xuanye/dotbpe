using DotBPE.Rpc.Exceptions;

namespace DotBPE.Rpc.Utils
{
    public static class Assert
    {
        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                throw new RpcBizException();
            }
        }


        public static void IsTrue(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new RpcBizException(errorMessage);
            }
        }


        public static void IsNull<T>(T reference)
        {
            if (reference == null)
            {
                throw new RpcBizException();
            }
        }


        public static void IsNull<T>(T reference, string errorMessage)
        {
            if (reference == null)
            {
                throw new RpcBizException(errorMessage);
            }
        }

        public static void IsNotNull<T>(T reference, string errorMessage)
        {
            if (reference != null)
            {
                throw new RpcBizException(errorMessage);
            }
        }
    }
}
