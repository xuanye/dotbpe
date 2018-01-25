set -ex

cd $(dirname $0)/../../src/

artifactsFolder="../artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet restore ./DotBPE.sln
dotnet build ./DotBPE.sln -c Release



versionNumber="1.1.10"


dotnet pack ./core/DotBPE.Rpc/DotBPE.Rpc.csproj -c Release -o ../../$artifactsFolder --version-suffix=$versionNumber
dotnet pack ./core/DotBPE.Rpc.Netty/DotBPE.Rpc.Netty.csproj -c Release -o ../../$artifactsFolder --version-suffix=$versionNumber
dotnet pack ./protocol/DotBPE.Protocol.Amp/DotBPE.Protocol.Amp.csproj -c Release -o ../../$artifactsFolder --version-suffix=$versionNumber
dotnet pack ./protocol/DotBPE.Protobuf/DotBPE.Protobuf.csproj -c Release -o ../../$artifactsFolder --version-suffix=$versionNumber


dotnet nuget push ./$artifactsFolder/DotBPE.Rpc.${versionNumber}.nupkg -k $NUGET_KEY -s https://www.nuget.org
dotnet nuget push ./$artifactsFolder/DotBPE.Rpc.Netty.${versionNumber}.nupkg -k $NUGET_KEY -s https://www.nuget.org
dotnet nuget push ./$artifactsFolder/DotBPE.Protocol.Amp.${versionNumber}.nupkg -k $NUGET_KEY -s https://www.nuget.org
dotnet nuget push ./$artifactsFolder/DotBPE.Protobuf.${versionNumber}.nupkg -k $NUGET_KEY -s https://www.nuget.org


