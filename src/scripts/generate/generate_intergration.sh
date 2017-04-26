set -ex

cd $(dirname $0)/../../test/IntegrationTesting/

PROTOC=protoc
PLUGIN=protoc-gen-dotbpe=../../tool/ampplugin/Protobuf.Gen.exe
IntegrationTesting_DIR=./DotBPE.IntegrationTesting/
PROTO_DIR=../../protos

$PROTOC  -I=$PROTO_DIR --csharp_out=$IntegrationTesting_DIR --dotbpe_out=$IntegrationTesting_DIR \
    $PROTO_DIR/{dotbpe_option,benchmark}.proto  --plugin=$PLUGIN
