set -ex

cd $(dirname $0)/../../artifacts/IntegrationTesting

# 性能测试
dotnet ./client/DotBPE.IntegrationTesting.Client.dll --server 127.0.0.1:6201 --testcase benchmark --rtc 8  --rc 500 --mpc 5

# 并发情况下的callcontext test
dotnet ./client/DotBPE.IntegrationTesting.Client.dll --server 127.0.0.1:6201 --testcase callcontexttest --rtc 5  --rc 500 --mpc 5
