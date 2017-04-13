using Google.Protobuf.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Protobuf;

namespace DotBPE.ProtobufPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
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
           DotbpeGen.Generate(request,response);
        }
    }
}
