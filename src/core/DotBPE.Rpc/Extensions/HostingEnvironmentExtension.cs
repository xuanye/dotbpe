using DotBPE.Rpc.Hosting;

namespace DotBPE.Rpc.Extensions
{
    public static class HostingEnvironmentExtension
    {
        public static IHostingEnvironment Initialize(this IHostingEnvironment env,string appName,RpcHostOption option){

            env.ApplicationName = appName;
            env.EnvironmentName  = option.EnvironmentName;
            return env;
        }
    }
}
