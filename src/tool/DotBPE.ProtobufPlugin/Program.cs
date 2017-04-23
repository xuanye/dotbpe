using Google.Protobuf;
using Google.Protobuf.Compiler;
using System;

namespace DotBPE.ProtobufPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var response = new CodeGeneratorResponse();
            try
            {
                CodeGeneratorRequest request;
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
