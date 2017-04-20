using System;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Protocol.Amp;
using Google.Protobuf;

namespace DotBPE.IntegrationTesting
{
    public class BenchmarkServerImple:BenchmarkTestBase
    {
        public override Task<BenchmarkMessage> EchoAsync(BenchmarkMessage request){
            return null;
        }
    }
}