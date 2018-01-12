using System;

namespace DotBPE.Rpc.Logging
{
    /// <summary>Logger that logs to System.Console.</summary>
    public class ConsoleLogger : TextWriterLogger
    {
        /// <summary>Creates a console logger not associated to any specific type.</summary>
        public ConsoleLogger() : this(null)
        {
        }

        /// <summary>Creates a console logger that logs messsage specific for given type.</summary>
        private ConsoleLogger(Type forType) : base(() => Console.Error, forType)
        {
        }

        /// <summary>
        /// Returns a logger associated with the specified type.
        /// </summary>
        public override ILogger ForType<T>()
        {
            if (typeof(T) == AssociatedType)
            {
                return this;
            }
            return new ConsoleLogger(typeof(T));
        }
    }
}
