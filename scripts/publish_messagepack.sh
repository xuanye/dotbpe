set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet build ./src/DotBPE.Extensions.MessagePack/DotBPE.Extensions.MessagePack.csproj -c Release

dotnet pack ./src/DotBPE.Extensions.MessagePack/DotBPE.Extensions.MessagePack.csproj -c Release -o $artifactsFolder

dotnet nuget push ./$artifactsFolder/DotBPE.Extensions.MessagePack.*.nupkg -k $NUGET_KEY -s https://www.nuget.org
