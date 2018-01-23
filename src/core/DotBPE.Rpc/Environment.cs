using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.Logging;
using System;

namespace DotBPE.Rpc
{
    public class Environment
    {
        private static ILoggerFactory _factory;

        /// <summary>
        /// Gets application-wide logger used by gRPC.
        /// </summary>
        /// <value>The logger.</value>
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                return _factory;
            }
        }

      

        /// <summary>
        /// Sets the application-wide logger that should be used by gRPC.
        /// </summary>
        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            Preconditions.CheckNotNull(loggerFactory, "loggerFactory");
            if(_factory == null)
                _factory = loggerFactory;
        }

    }
}
