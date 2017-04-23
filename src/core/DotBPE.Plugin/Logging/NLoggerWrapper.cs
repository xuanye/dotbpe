using System;
using DotBPE.Rpc.Logging;

namespace DotBPE.Plugin.Logging
{
    public class NLoggerWrapper: ILogger
    {
        readonly Type _forType;
        private readonly NLog.ILogger _logger;
        public NLoggerWrapper():this(typeof(NLoggerWrapper))
        {
            
        }
        public NLoggerWrapper(Type forType)
        {
            this._forType = forType;
            this._logger = NLog.LogManager.GetLogger(this._forType.FullName);
        }

        /// <summary>
        /// Returns a logger associated with the specified type.
        /// </summary>
        public virtual ILogger ForType<T>()
        {
            if (typeof(T) == this._forType)
            {
                return this;
            }
            return new NLoggerWrapper( typeof(T));
        }

        public void Debug(string message)
        {
            this._logger.Debug(message);
        }

        public void Debug(string format, params object[] formatArgs)
        {
            this._logger.Debug(format, formatArgs);
        }

        public void Info(string message)
        {
            this._logger.Info(message);
        }

        public void Info(string format, params object[] formatArgs)
        {
            this._logger.Info(format, formatArgs);
        }

        public void Warning(string message)
        {
            this._logger.Warn(message);
        }

        public void Warning(string format, params object[] formatArgs)
        {
            this._logger.Warn(format, formatArgs);
        }

        public void Warning(Exception exception, string message)
        {
            this._logger.Warn(exception, message); 
        }

        public void Error(string message)
        {
            this._logger.Error(message);
        }

        public void Error(string format, params object[] formatArgs)
        {
            this._logger.Error(format, formatArgs);
        }

        public void Error(Exception exception, string message)
        {
            this._logger.Error(exception, message);
        }
    }
}
