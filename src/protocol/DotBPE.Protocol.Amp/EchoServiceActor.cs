using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Rpc;

namespace DotBPE.Protocol.Amp
{
    public class EchoServiceActor : IServiceActor<AmpMessage>
    {
        public string Id => "100$1";

        public async Task Receive(IRpcContext<AmpMessage> context, AmpMessage message)
        {
            Console.WriteLine($"收到消息：{Encoding.UTF8.GetString(message.Data)}");
            await context.SendAsync(message);
        }
    }
}
