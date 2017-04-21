set -ex

cd $(dirname $0)/../../../artifacts/IntegrationTesting

dotnet exec ./client/DotBPE.IntegrationTesting.Client.dll --server 127.0.0.1:6201 --testcase benchmark --rtc 2  --rc 50000 --mpc 8
