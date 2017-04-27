namespace DotBPE.Rpc
{
    public interface IHostingEnvironment
    {
        /// <summary>
        ///  应用名称
        /// </summary>
        /// <returns></returns>
        string ApplicationName{ get;set;}

        string AppRoot{get;}

        string EnvironmentName {get;set;}


        bool IsDevelopment();
    }
}
