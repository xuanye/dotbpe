namespace DotBPE.Rpc.Utils
{
    public static class CommonUtils
    {
        public static string GetAppRootPath(){
            #if DOTNETCORE
                var rootPath = System.AppContext.BaseDirectory;
            #else
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            #endif

            return rootPath;
        }
    }
}
