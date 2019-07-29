using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tomato.Rpc.Client
{

    public class DefaultClientAuditLoggerFactory:IClientAuditLoggerFactory
    {
        private readonly IServiceProvider _provider;

        public DefaultClientAuditLoggerFactory(IServiceProvider provider)
        {
            this._provider = provider;
        }



        public virtual IClientAuditLogger GetLogger(string methodFullName)
        {
            var logger = this._provider.GetService<IClientAuditLogger>() ?? NullClientAuditLogger.Instance;
            return logger;
        }
    }

    public class NullClientAuditLogger : IClientAuditLogger
    {
        public static readonly NullClientAuditLogger Instance = new NullClientAuditLogger();


        public string MethodFullName { get; }

        public void SetParameter(object parameter)
        {
        }

        public void SetReturnValue(object retVal)
        {
        }

        public void Dispose()
        {

        }
    }
}
