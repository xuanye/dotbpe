set -ex

cd $(dirname $0)/../../src/sample/HelloRpc/

PROTOC=protoc
PLUGIN=protoc-gen-dotbpe=../../tool/ampplugin/Protobuf.Gen.exe
HELLORPC_DIR=./HelloRpc.Common/
PROTO_DIR=../../protos

$PROTOC  -I=$PROTO_DIR --csharp_out=$HELLORPC_DIR --dotbpe_out=$HELLORPC_DIR \
    $PROTO_DIR/{dotbpe_option,hello_rpc}.proto  --plugin=$PLUGIN
