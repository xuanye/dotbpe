using System;
using System.IO;
using Google.Protobuf;
using Google.Protobuf.Compiler;

namespace Protobuff.Plugin
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream inStream = Console.OpenStandardInput();

            var request = CodeGeneratorRequest.Parser.ParseDelimitedFrom(inStream);
            var response = new CodeGeneratorResponse();

            ParseCode(request, response);

            var codesArr =response.ToByteArray();

            Stream outStream =  Console.OpenStandardOutput();

            outStream.Write(codesArr, 0, codesArr.Length);
        }

        private static void ParseCode(CodeGeneratorRequest request, CodeGeneratorResponse response)
        {
            
        }
    }
}