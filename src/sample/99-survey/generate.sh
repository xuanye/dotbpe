set -ex

cd $(dirname $0)

PROTOC=protoc
PLUGIN=protoc-gen-dotbpe=../../tool/ampplugin/Protobuf.Gen.exe
OUT_DIR=./Survey.Core/_g
PROTO_DIR=./protos
PROTO_BASE_DIR=../../protos

if [ -d $OUT_DIR ]; then
  rm -rf $OUT_DIR
fi

mkdir -p $OUT_DIR

$PROTOC -I=$PROTO_BASE_DIR -I=$PROTO_DIR  --csharp_out=$OUT_DIR --dotbpe_out=$OUT_DIR \
  $PROTO_DIR/{message/common,message/apaper,message/qpaper,message/user}.proto  $PROTO_DIR/service/{gate/*,inner/*}.proto  --plugin=$PLUGIN
