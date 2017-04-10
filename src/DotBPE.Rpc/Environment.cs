using DotBPE.Rpc.Logging;
using DotBPE.Rpc.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    public class Environment
    {
        static ILogger logger = new NullLogger();


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

        /// <summary>
        /// Sets the application-wide logger that should be used by gRPC.
        /// </summary>
        public static void SetLogger(ILogger customLogger)
        {
            Preconditions.CheckNotNull(customLogger, "customLogger");
            logger = customLogger;
        }
    }
}
