set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder


dotnet pack ./src/DotBPE.Gateway/DotBPE.Gateway.csproj -c Release -o ../../$artifactsFolder

dotnet nuget push ./$artifactsFolder/DotBPE.Gateway.*.nupkg -k $NUGET_KEY -s https://www.nuget.org
