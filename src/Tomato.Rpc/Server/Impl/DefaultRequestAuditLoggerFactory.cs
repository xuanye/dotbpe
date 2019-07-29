using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tomato.Rpc.Server
{
    /// <inheritdoc />
    public class DefaultRequestAuditLoggerFactory:IRequestAuditLoggerFactory
    {

        private readonly IServiceProvider _provider;

        public DefaultRequestAuditLoggerFactory(IServiceProvider provider)
        {
            this._provider = provider;
        }

        public IRequestAuditLogger GetLogger(string methodFullName)
        {
            var logger = this._provider.GetService<IRequestAuditLogger>() ?? NullRequestAuditLogger.Instance;
            return logger;
        }


    }

    /// <inheritdoc />
    public class NullRequestAuditLogger : IRequestAuditLogger
    {
        public static readonly NullRequestAuditLogger Instance = new NullRequestAuditLogger();


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

        public void SetContext(IRpcContext context)
        {
           
        }
    }

}
