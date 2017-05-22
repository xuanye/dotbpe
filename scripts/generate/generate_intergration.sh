set -ex

cd $(dirname $0)/../../src/test/IntegrationTesting/

PROTOC=protoc
PLUGIN=protoc-gen-dotbpe=../../tool/ampplugin/Protobuf.Gen.exe
IntegrationTesting_DIR=./DotBPE.IntegrationTesting/_g
PROTO_DIR=../../protos


if [ -d $IntegrationTesting_DIR ]; then
  rm -rf $IntegrationTesting_DIR
fi

mkdir -p $IntegrationTesting_DIR


$PROTOC  -I=$PROTO_DIR --csharp_out=$IntegrationTesting_DIR --dotbpe_out=$IntegrationTesting_DIR \
    $PROTO_DIR/{dotbpe_option,benchmark,callcontext_test}.proto  --plugin=$PLUGIN
