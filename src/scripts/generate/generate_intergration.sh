set -ex

cd $(dirname $0)/../../test/IntegrationTesting/

PROTOC=protoc
PLUGIN=protoc-gen-dotbpe=../../tool/ampplugin/dotbpe_amp.exe
IntegrationTesting_DIR=./DotBPE.IntegrationTesting/


$PROTOC  -I=./protos --csharp_out=$IntegrationTesting_DIR --dotbpe_out=$IntegrationTesting_DIR \
    ./protos/{dotbpe_option,benchmark}.proto  --plugin=$PLUGIN
