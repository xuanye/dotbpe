set -ex

cd $(dirname $0)/../../artifacts/IntegrationTesting

dotnet exec ./client/DotBPE.IntegrationTesting.Client.dll --server 172.17.0.2:6201 --testcase benchmark --rtc 1  --rc 1000 --mpc 8
