set -ex

cd $(dirname $0)/../../src/test/IntegrationTesting/DotBPE.IntegrationTesting.QpsServer/

dotnet publish ./DotBPE.IntegrationTesting.QpsServer.csproj -o ./bin/out -c Release
docker build -t qpsserver ./
