using System;
using DotBPE.Rpc.Utils;

namespace DotBPE.Rpc.Hosting
{
    public class HostingEnvironment : IHostingEnvironment
    {
        public string ApplicationName { get;set; }
        public string AppRoot { get { return CommonUtils.GetAppRootPath(); }  }
        public string EnvironmentName { get;set; } = DotBPE.Rpc.Hosting.EnvironmentName.Production;

        public bool IsDevelopment()
        {
           return this.EnvironmentName ==  DotBPE.Rpc.Hosting.EnvironmentName.Development;
        }
    }
}
