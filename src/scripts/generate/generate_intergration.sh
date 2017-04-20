set -ex

cd $(dirname $0)/../../test/IntegrationTesting/

protoc  -I=./protos --csharp_out=./DotBPE.IntegrationTesting/ --dotbpe_out=./DotBPE.IntegrationTesting/ ./protos/dotbpe_option.proto ./protos/benchmark.proto --plugin=protoc-gen-dotbpe=../../tool/apmplugin/dotbpe_amp.exe