set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet build ./src/DotBPE.Extra.Pipeline/DotBPE.Extra.Pipeline.csproj -c Release

dotnet pack ./src/DotBPE.Extra.Pipeline/DotBPE.Extra.Pipeline.csproj -c Release -o $artifactsFolder

dotnet nuget push ./$artifactsFolder/DotBPE.Extra.Pipeline.*.nupkg -k $NUGET_KEY -s https://www.nuget.org
