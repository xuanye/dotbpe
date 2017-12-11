set -ex

PROTOC=protoc

OUT_DIR=./MathCommon/_g
PROTO_DIR=./proto


if [ -d $OUT_DIR ]; then
  rm -rf $OUT_DIR
fi

mkdir -p $OUT_DIR


$PROTOC  -I=$PROTO_DIR --csharp_out=$OUT_DIR  $PROTO_DIR/math.proto
