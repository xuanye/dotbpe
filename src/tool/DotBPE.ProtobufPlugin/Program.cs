using Google.Protobuf.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Newtonsoft.Json;

namespace DotBPE.ProtobufPlugin
{
    class Program
    {
        static void Main(string[] args)
        {

            CodeGeneratorRequest request;
            var response = new CodeGeneratorResponse();
            try
            {
                using (var inStream = Console.OpenStandardInput())
                {
                    request = CodeGeneratorRequest.Parser.ParseFrom(inStream);
                }
                ParseCode(request, response);
            }
            catch (Exception e)
            {
                response.Error += e.ToString();
            }

            using (var output = Console.OpenStandardOutput())
            {
                //byte[] bytea= response.ToByteArray();
                //response.Error += Encoding.UTF8.GetString(bytea);
                response.WriteTo(output);
                output.Flush();

            }
        }
        private static void ParseCode(CodeGeneratorRequest request, CodeGeneratorResponse response)
        {
            List<string> slist = new List<string>();
            foreach( var protofile in request.ProtoFile)
            {
                var nfile = new CodeGeneratorResponse.Types.File();
                nfile.Name = protofile.Name + ".json";

                string content = "";
                foreach(var service in protofile.Service)
                {
                    int serviceId;
                    service.Options.CustomOptions.TryGetInt32(10000, out serviceId);
                    content = string.Format("serviceName={0},serviceId={1}", service.Name, serviceId);

                }
               
                nfile.Content = content;
                response.File.Add(nfile);
            }
        }
    }
}
