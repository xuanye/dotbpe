
using DotBPE.Rpc.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace DotBPE.Rpc.Logging
{
    /// <summary>Logger that filters out messages below certain log level.</summary>
    public class LogLevelFilterLogger : ILogger
    {
        readonly ILogger innerLogger;
        readonly LogLevel logLevel;

        /// <summary>
        /// Creates and instance of <c>LogLevelFilter.</c>
        /// </summary>
        public LogLevelFilterLogger(ILogger logger, LogLevel logLevel)
        {
            this.innerLogger = Preconditions.CheckNotNull(logger);
            this.logLevel = logLevel;
        }

        /// <summary>
        /// Returns a logger associated with the specified type.
        /// </summary>
        public virtual ILogger ForType<T>()
        {
            var newInnerLogger = innerLogger.ForType<T>();
            if (object.ReferenceEquals(this.innerLogger, newInnerLogger))
            {
                return this;
            }
            return new LogLevelFilterLogger(newInnerLogger, logLevel);
        }

        /// <summary>Logs a message with severity Debug.</summary>
        public void Debug(string message)
        {
            if (logLevel <= LogLevel.Debug)
            {
                innerLogger.Debug(message);
            }
        }

        /// <summary>Logs a formatted message with severity Debug.</summary>
        public void Debug(string format, params object[] formatArgs)
        {
            if (logLevel <= LogLevel.Debug)
            {
                innerLogger.Debug(format, formatArgs);
            }
        }

        /// <summary>Logs a message with severity Info.</summary>
        public void Info(string message)
        {
            if (logLevel <= LogLevel.Info)
            {
                innerLogger.Info(message);
            }
        }

        /// <summary>Logs a formatted message with severity Info.</summary>
        public void Info(string format, params object[] formatArgs)
        {
            if (logLevel <= LogLevel.Info)
            {
                innerLogger.Info(format, formatArgs);
            }
        }

        /// <summary>Logs a message with severity Warning.</summary>
        public void Warning(string message)
        {
            if (logLevel <= LogLevel.Warning)
            {
                innerLogger.Warning(message);
            }
        }

        /// <summary>Logs a formatted message with severity Warning.</summary>
        public void Warning(string format, params object[] formatArgs)
        {
            if (logLevel <= LogLevel.Warning)
            {
                innerLogger.Warning(format, formatArgs);
            }
        }

        /// <summary>Logs a message and an associated exception with severity Warning.</summary>
        public void Warning(Exception exception, string message)
        {
            if (logLevel <= LogLevel.Warning)
            {
                innerLogger.Warning(exception, message);
            }
        }

        /// <summary>Logs a message with severity Error.</summary>
        public void Error(string message)
        {
            if (logLevel <= LogLevel.Error)
            {
                innerLogger.Error(message);
            }
        }

        /// <summary>Logs a formatted message with severity Error.</summary>
        public void Error(string format, params object[] formatArgs)
        {
            if (logLevel <= LogLevel.Error)
            {
                innerLogger.Error(format, formatArgs);
            }
        }

        /// <summary>Logs a message and an associated exception with severity Error.</summary>
        public void Error(Exception exception, string message)
        {
            if (logLevel <= LogLevel.Error)
            {
                innerLogger.Error(exception, message);
            }
        }
    }
}
