protoc  -I=./protos --csharp_out=./HelloRpc.Common/ --dotbpe_out=./HelloRpc.Common/  ./protos/HelloRpc.proto --plugin=protoc-gen-dotbpe=./plugins/DotBPE.ProtobufPlugin.exe
