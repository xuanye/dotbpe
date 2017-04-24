namespace DotBPE.Rpc.Utils
{
    public static class CommonUtils
    {
        public static string GetAppRootPath(){
            #if NET4
                    var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            #else
                    var rootPath = System.AppContext.BaseDirectory;
            #endif

            return rootPath;
        }
    }
}
