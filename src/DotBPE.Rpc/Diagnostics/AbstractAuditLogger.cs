using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using DotBPE.Baseline.Extensions;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Internal;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Peach;

namespace DotBPE.Rpc
{
    public abstract class AbstractAuditLogger : IDisposable
    {

        public IRpcContext Context { get; set; }
        public abstract string MethodFullName { get;}

        protected class AuditLogEntity
        {
            public object Request { get; set; }

            public object Response { get; set; }

            public AuditLogType LogType { get; set; }

            public long ElapsedMS { get; set; }

            public string MethodFullName { get; set; }

            public IRpcContext Context { get; set; }

            public ILogger Writer { get; set; }

            public IAuditLoggerFormat Formatter { get; set; }
        }

        private static readonly ConcurrentQueue<AuditLogEntity> logDict = new ConcurrentQueue<AuditLogEntity>();

        private static bool _isRunning;

        private Stopwatch _sw;
        private object _req;
        private object _rsp;


        protected AbstractAuditLogger()
        {
            _sw = Stopwatch.StartNew();
        }

        private static IAuditLoggerFormat _formatter;

        private IAuditLoggerFormat Formatter
        {
            get
            {
                if (_formatter == null)
                {
                    if (Internal.Environment.ServiceProvider != null)
                    {
                        _formatter = Internal.Environment.ServiceProvider.GetService<IAuditLoggerFormat>();
                    }
                }
                return _formatter;
            }
        }

        private static void AddAuditLog(IRpcContext context,ILogger writer, IAuditLoggerFormat format, AuditLogType logType,string methodName,  object req, object rsp, long elapsedMS)
        {
            if (writer == null || format == null)
            {
                return;
            }

            var entity = new AuditLogEntity{
                MethodFullName = methodName,
                Request = req,
                Response = rsp,
                ElapsedMS = elapsedMS,
                Writer = writer,
                Context =  context,
                Formatter = format,
                LogType = logType
            };

            logDict.Enqueue(entity);

            StartWrite();
        }

        private static void StartWrite()
        {
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;

            Task.Factory.StartNew(WriteLog, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        private static async Task WriteLog()
        {
            await  Task.Delay(3000);
            while (!logDict.IsEmpty)
            {
                if (!logDict.TryDequeue(out var log))
                {
                    continue;
                }
                try
                {
                    if (log.Formatter != null && log.Writer != null)
                    {
                        var realRet =  await DrillDownResponseObj(log.Response);
                        var logText = log.Formatter.Format(log.Context,log.LogType,log.MethodFullName, log.Request, realRet, log.ElapsedMS);
                        if (!string.IsNullOrEmpty(logText))
                        {
                            log.Writer.LogInformation(logText);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--write audit logger error--" + ex.Message + "-----\r\n-----" + ex.StackTrace);
                    //无能为力了
                }
            }
            _isRunning = false;
        }

        private static Task<RpcResult<object>> DrillDownResponseObj(object retVal)
        {
            return InternalHelper.DrillDownResponseObj(retVal);
        }

        public void Dispose()
        {
            _sw.Stop();
            long es = _sw.ElapsedMilliseconds;
            if (this._req != null && this._rsp != null)
            {
                AddAuditLog(this.Context,GetTypeLogger(), GetLoggerFormat(), GetAuditLogType(),this.MethodFullName,  this._req, this._rsp, es);
            }
            _sw = null;
        }


        public void SetParameter(object parameter)
        {
            _req = parameter;
        }

        public void SetReturnValue(object retVal)
        {
            _rsp = retVal;
        }


        protected abstract AuditLogType GetAuditLogType();

        protected abstract ILogger GetTypeLogger();

        protected virtual IAuditLoggerFormat GetLoggerFormat()
        {
            return this.Formatter;
        }
    }
}
