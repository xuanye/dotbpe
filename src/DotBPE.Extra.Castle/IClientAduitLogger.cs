namespace DotBPE.Extra.Castle
{
    using System;
    public interface IClientAduitLogger:IDisposable
    {
        void SetServiceName(string serviceName);
        void SetRequest(object req);
        void SetResponse(object res);
    }

    public class NullClientAduitLogger : IClientAduitLogger
    {
        public static readonly NullClientAduitLogger Instance = new NullClientAduitLogger();



        public void Dispose()
        {

        }

        public void SetRequest(object req)
        {

        }

        public void SetResponse(object res)
        {

        }

        public void SetServiceName(string serviceName)
        {

        }
    }
}
