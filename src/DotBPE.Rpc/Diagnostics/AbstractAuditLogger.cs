using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using DotBPE.Baseline.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc
{
    public abstract class AbstractAuditLogger : IDisposable
    {
        protected class AuditLogEntity
        {
            public object Request { get; set; }

            public object Response { get; set; }

            public AuditLogType LogType { get; set; }

            public long ElapsedMS { get; set; }

            public string MethodFullName { get; set; }

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

        private static ISerializer _Serializer;
        private static ISerializer Serializer
        {
            get
            {
                if (_Serializer == null)
                {
                    if (Internal.Environment.ServiceProvider != null)
                    {
                        _Serializer = Internal.Environment.ServiceProvider.GetService<ISerializer>();
                    }
                }
                return _Serializer;
            }
        }

        private static void AddAuditLog(ILogger writer, IAuditLoggerFormat format, AuditLogType logType,  object req, object rsp, long elapsedMS)
        {
            if (writer == null || format == null)
            {
                return;
            }

            var entity = new AuditLogEntity{

                Request = req,
                Response = rsp,
                ElapsedMS = elapsedMS,
                Writer = writer,
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

            Task.Factory.StartNew(WriteLog, TaskCreationOptions.LongRunning);
        }

        private static async Task WriteLog()
        {
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
                        var logText = log.Formatter.Format(log.LogType,log.MethodFullName, log.Request, realRet, log.ElapsedMS);
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

        private static async Task<RpcResult<object>> DrillDownResponseObj(object retVal)
        {
            var result = new RpcResult<object>();
            var retValType = retVal.GetType();
            if (retValType == typeof(Task))
            {
                return result;
            }


            var tType = retValType.GenericTypeArguments[0];
            if (tType == typeof(RpcResult))
            {
                var retTask = retVal as Task<RpcResult>;
                var tmp = await retTask;
                result.Code = tmp.Code;
                return result;
            }

            if (tType.IsGenericType)
            {
                Task retTask = retVal as Task;
                await retTask.AnyContext();

                var resultProp = retValType.GetProperty("Result");
                if (resultProp == null)
                {
                    result.Code = RpcErrorCodes.CODE_INTERNAL_ERROR;
                    return result;
                }

                object realVal = resultProp.GetValue(retVal);

                object dataVal = null;
                var dataProp = tType.GetProperty("Data");
                if (dataProp != null)
                {
                    dataVal = dataProp.GetValue(realVal);
                }

                if (dataVal != null)
                {
                    result.Data = Serializer.Serialize(dataVal);
                }
            }

            return null;
        }

        public void Dispose()
        {
            _sw.Stop();
            long es = _sw.ElapsedMilliseconds;
            if (this._req != null && this._rsp != null)
            {
                AddAuditLog(GetTypeLogger(), GetLoggerFormat(), GetAuditLogType(),  this._req, this._rsp, es);
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
