
namespace DotBPE.IntegrationTesting.QpsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                args = new string[] { "--port","6201" };
            }
            DotBPE.Rpc.Environment.SetLogger(new DotBPE.Rpc.Logging.ConsoleLogger());
            QpsServerWorker.Run(args);
        }
    }
}
