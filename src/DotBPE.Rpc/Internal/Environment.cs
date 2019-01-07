using Microsoft.Extensions.Logging;
using Peach.Infrastructure;
using System;
using DotBPE.Rpc.Codec;

namespace DotBPE.Rpc.Internal
{
    public static class Environment
    {
        /// <summary>
        /// Gets application-wide logger used by internal.
        /// </summary>
        /// <value>The logger.</value>
        public static ILoggerFactory LoggerFactory { get; private set; }

        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Sets the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            Preconditions.CheckNotNull(serviceProvider, nameof(serviceProvider));
            if (ServiceProvider == null)
                ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Sets the application-wide logger that should be used by internal.
        /// </summary>
        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            Preconditions.CheckNotNull(loggerFactory, nameof(LoggerFactory));
            if (LoggerFactory == null)
                LoggerFactory = loggerFactory;
        }

        public static string GetAppBasePath()
        {
            return AppContext.BaseDirectory;
            //return AppDomain.CurrentDomain.BaseDirectory;
        }

    }
}
