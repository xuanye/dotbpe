set -ex

cd $(dirname $0)/../../src/test/IntegrationTesting/

BUILD_DIR=../../../../artifacts/IntegrationTesting/


dotnet build ./DotBPE.IntegrationTesting.Client/DotBPE.IntegrationTesting.Client.csproj -c Release
dotnet build ./DotBPE.IntegrationTesting.QpsServer/DotBPE.IntegrationTesting.QpsServer.csproj -c Release
