set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet restore ./DotBPE.sln
dotnet build ./DotBPE.sln -c Release

dotnet pack ./src/DotBPE.Baseline/DotBPE.Baseline.csproj -c Release -o $artifactsFolder
dotnet pack ./src/DotBPE.Extra.Castle/DotBPE.Extra.Castle.csproj -c Release -o $artifactsFolder
dotnet pack ./src/DotBPE.Extra.JsonNet/DotBPE.Extra.JsonNet.csproj -c Release -o $artifactsFolder
dotnet pack ./src/DotBPE.Extra.MessagePack/DotBPE.Extra.MessagePack.csproj -c Release -o $artifactsFolder
dotnet pack ./src/DotBPE.Extra.Protobuf/DotBPE.Extra.Protobuf.csproj -c Release -o $artifactsFolder
dotnet pack ./src/DotBPE.Extra.Json/DotBPE.Extra.Json.csproj -c Release -o $artifactsFolder

dotnet pack ./src/DotBPE.Gateway/DotBPE.Gateway.csproj -c Release -o $artifactsFolder
dotnet pack ./src/DotBPE.Rpc/DotBPE.Rpc.csproj -c Release -o $artifactsFolder
dotnet pack ./src/DotBPE.BestPractice/DotBPE.BestPractice.csproj -c Release -o $artifactsFolder

