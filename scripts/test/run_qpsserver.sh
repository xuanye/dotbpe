set -ex

cd $(dirname $0)/../../artifacts/IntegrationTesting

dotnet exec ./qpsserver/DotBPE.IntegrationTesting.QpsServer.dll --port 6201
