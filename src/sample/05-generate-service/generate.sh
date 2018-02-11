set -ex

cd $(dirname $0)

PROTOC=protoc
PLUGIN=protoc-gen-dotbpe=../../tool/ampplugin/Protobuf.Gen.exe,1
OUT_DIR=./MathCommon/_g
PROTO_DIR=./protos
PROTO_BASE_DIR=../../protos

if [ -d $OUT_DIR ]; then
  rm -rf $OUT_DIR
fi

mkdir -p $OUT_DIR

$PROTOC -I=$PROTO_BASE_DIR -I=$PROTO_DIR  --csharp_out=$OUT_DIR --dotbpe_out=$OUT_DIR \
    $PROTO_DIR/math.proto  --plugin=$PLUGIN
