using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Peach.Buffer;
using Peach.Messaging;

namespace Peach.Protocol
{
    public class CommandLineProtocol : IProtocol<CommandLineMessage>
    {
        const string SPLITER = " ";
        public ProtocolMeta GetProtocolMeta()
        {
            return null; //Simple Protocol
        }

        public void Pack(IBufferWriter writer, CommandLineMessage message)
        {           
            string content = string.Format("{0}{1}{2}", message.Command, SPLITER, string.Join(SPLITER, message.Parameters));
            writer.WriteBytes(Encoding.UTF8.GetBytes(content));
        }

        public void PackHeartBeat(IBufferWriter writer)
        {
            Pack(writer, new CommandLineMessage("heartbeat"));
        }

        public CommandLineMessage Parse(IBufferReader reader)
        {
            byte[] buffer = new byte[reader.ReadableBytes];
            reader.ReadBytes(buffer);
            string content = Encoding.UTF8.GetString(buffer);

            var arr = content.Split(new string[] { SPLITER }, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length == 0) return new CommandLineMessage(string.Empty);
            if (arr.Length == 1) return new CommandLineMessage(arr[0]);

            return new CommandLineMessage(arr[0], arr.Skip(1).ToArray());
        }
    }
}
