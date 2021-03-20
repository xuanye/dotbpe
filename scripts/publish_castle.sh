set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet build ./src/DotBPE.Extra.Castle/DotBPE.Extra.Castle.csproj -c Release

dotnet pack ./src/DotBPE.Extra.Castle/DotBPE.Extra.Castle.csproj -c Release -o $artifactsFolder

dotnet nuget push ./$artifactsFolder/DotBPE.Extra.Castle.*.nupkg -k $NUGET_KEY -s https://www.nuget.org
