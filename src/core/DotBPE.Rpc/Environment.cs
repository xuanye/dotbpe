using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Logging;
using DotBPE.Rpc.Utils;
using System;

namespace DotBPE.Rpc
{
    public class Environment
    {
        static ILogger logger = new NullLogger();

        static IServiceProvider serviceProvider = null;

        /// <summary>
        /// Gets application-wide logger used by gRPC.
        /// </summary>
        /// <value>The logger.</value>
        public static ILogger Logger
        {
            get
            {
                return logger;
            }
        }

        public static IServiceProvider ServiceProvider {

            get
            {
                if(serviceProvider == null)
                {
                    throw new RpcException("ServiceProvider 未设置，请检查程序是否正确启动");
                }
                return serviceProvider;
            }
        }


        /// <summary>
        /// Sets the application-wide logger that should be used by gRPC.
        /// </summary>
        public static void SetLogger(ILogger customLogger)
        {
            Preconditions.CheckNotNull(customLogger, "customLogger");
            logger = customLogger;
        }

        /// <summary>
        /// 设置当前环境的ServiceProvider
        /// </summary>
        /// <param name="provider"></param>
        public static void SetServiceProvider(IServiceProvider provider)
        {
            Preconditions.CheckNotNull(provider, "provider");
            serviceProvider = provider;
        }
    }
}
