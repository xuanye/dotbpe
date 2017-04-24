using DotBPE.Rpc;
using DotBPE.Plugin;
using Xunit;
using DotBPE.Plugin.Logging;

namespace DotBPE.UnitTest.Plugin
{
    public class NLogTestcs
    {
        [Fact]
        public void TestNLogWrapper(){
            Environment.SetLogger(new NLoggerWrapper(typeof(NLogTestcs)));
        }
    }
}
