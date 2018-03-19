using DotBPE.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DotBPE.Protocol.Amp
{
    public abstract class AbstractAuditLogger:IDisposable
    {
        protected class AuditLogEntity
        {
            public AmpMessage Request { get; set; }

            public AmpMessage Response { get; set; }

            public AuditLogType LogType { get; set; }

            public long ElapsedMS { get; set; }

            public IRpcContext Context { get; set; }

            public ILogger Writer { get; set; }

            public IAuditLoggerFormat<AmpMessage> Formater { get;  set; }
        }



        private static Queue<AuditLogEntity> logDict = new Queue<AuditLogEntity>();
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
        protected  IAuditLoggerFormat<AmpMessage> Formater
        {
            get
            {
                if(_Formater == null)
                {
                    if (Rpc.Environment.ServiceProvider != null)
                    {
                        _Formater = Rpc.Environment.ServiceProvider.GetService<IAuditLoggerFormat<AmpMessage>>();
                    }
                }
                return _Formater;
            }
        }

        private static void AddAuditLog(ILogger writer,IAuditLoggerFormat<AmpMessage> format,AuditLogType logType,IRpcContext context, AmpMessage req, AmpMessage rsp, long elapsedMS)
        {
            if(writer == null || format == null)
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
            lock (lockobject)
            {
                logDict.Enqueue(entity);
            }
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
            while (logDict.Count > 0)
            {
                var log = logDict.Dequeue();
                try
                {
                    if (log.Formater != null)
                    {
                        string logText = log.Formater.Format(log.Context,log.LogType,log.Request, log.Response, log.ElapsedMS);
                        if(!string.IsNullOrEmpty(logText))
                        {
                            log.Writer.LogInformation(logText);
                        }

                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message+"\r"+ex.StackTrace);
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
                AddAuditLog(GetTypeLogger(),GetLoggerFormat(),GetAuditLogType(), _context, this._req, this._rsp, es);
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
