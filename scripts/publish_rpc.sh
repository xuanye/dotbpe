set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet build ./src/DotBPE.Rpc/DotBPE.Rpc.csproj -c Release

dotnet pack ./src/DotBPE.Rpc/DotBPE.Rpc.csproj -c Release -o $artifactsFolder

dotnet nuget push ./$artifactsFolder/DotBPE.Rpc.*.nupkg -k $NUGET_KEY -s https://www.nuget.org
