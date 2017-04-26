set -ex

cd $(dirname $0)/../../src/test/IntegrationTesting/

BUILD_DIR=../../../../artifacts/IntegrationTesting/

mkdir -p $BUILD_DIR

dotnet publish ./DotBPE.IntegrationTesting.Client/DotBPE.IntegrationTesting.Client.csproj -o $BUILD_DIR/client -c Release
dotnet publish ./DotBPE.IntegrationTesting.QpsServer/DotBPE.IntegrationTesting.QpsServer.csproj -o $BUILD_DIR/qpsserver -c Release
