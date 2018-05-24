using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace DotBPE.Protocol.Amp
{
    public abstract class AbstractAuditLogger : IDisposable
    {
        protected class AuditLogEntity
        {
            public AmpMessage Request { get; set; }

            public AmpMessage Response { get; set; }

            public AuditLogType LogType { get; set; }

            public long ElapsedMS { get; set; }

            public IRpcContext Context { get; set; }

            public ILogger Writer { get; set; }

            public IAuditLoggerFormat<AmpMessage> Formater { get; set; }
        }

        private static ConcurrentQueue<AuditLogEntity> logDict = new ConcurrentQueue<AuditLogEntity>();
        private static object lockobject = new object();
        private static bool _isruning = false;

        private Stopwatch _sw;
        private AmpMessage _req;
        private AmpMessage _rsp;
        private IRpcContext _context;

        public AbstractAuditLogger()
        {
            _sw = Stopwatch.StartNew();
        }

        private static IAuditLoggerFormat<AmpMessage> _Formater;

        protected IAuditLoggerFormat<AmpMessage> Formater
        {
            get
            {
                if (_Formater == null)
                {
                    if (Rpc.Environment.ServiceProvider != null)
                    {
                        _Formater = Rpc.Environment.ServiceProvider.GetService<IAuditLoggerFormat<AmpMessage>>();
                    }
                }
                return _Formater;
            }
        }

        private static void AddAuditLog(ILogger writer, IAuditLoggerFormat<AmpMessage> format, AuditLogType logType, IRpcContext context, AmpMessage req, AmpMessage rsp, long elapsedMS)
        {
            if (writer == null || format == null)
            {
                return;
            }

            var entity = new AuditLogEntity()
            {
                Context = context,
                Request = req,
                Response = rsp,
                ElapsedMS = elapsedMS,
                Writer = writer,
                Formater = format,
                LogType = logType
            };

            logDict.Enqueue(entity);

            StartWrite();
        }

        private static void StartWrite()
        {
            if (_isruning)
            {
                return;
            }
            _isruning = true;
            Thread thread = new Thread(new ThreadStart(WriteLog));
            thread.IsBackground = true;
            thread.Start();
        }

        private static void WriteLog()
        {
            while (!logDict.IsEmpty)
            {
                AuditLogEntity log;
                var hasLog = logDict.TryDequeue(out log);
                if (!hasLog || log == null)
                {
                    return;
                }
                try
                {
                    if (log.Formater != null && log.Writer != null)
                    {
                        string logText = log.Formater.Format(log.Context, log.LogType, log.Request, log.Response, log.ElapsedMS);
                        if (!string.IsNullOrEmpty(logText))
                        {
                            log.Writer.LogInformation(logText);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--写日志出错了--" + ex.Message + "-----\r\n-----" + ex.StackTrace);
                    //无能为力了
                }
            }
            _isruning = false;
        }

        public void Dispose()
        {
            _sw.Stop();
            long es = _sw.ElapsedMilliseconds;
            if (this._req != null && this._rsp != null)
            {
                AddAuditLog(GetTypeLogger(), GetLoggerFormat(), GetAuditLogType(), _context, this._req, this._rsp, es);
            }
            _sw = null;
        }

        public void PushContext(IRpcContext context)
        {
            _context = context;
        }

        public void PushRequest(AmpMessage request)
        {
            _req = request;
        }

        public void PushResponse(AmpMessage response)
        {
            _rsp = response;
        }

        protected abstract AuditLogType GetAuditLogType();

        protected abstract ILogger GetTypeLogger();

        protected virtual IAuditLoggerFormat<AmpMessage> GetLoggerFormat()
        {
            return this.Formater;
        }
    }
}
