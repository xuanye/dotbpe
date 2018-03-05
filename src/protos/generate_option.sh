set -ex

cd $(dirname $0)

PROTOC=protoc

OUT_DIR=../protocol/DotBPE.Protobuf


PROTO_DIR=.

$PROTOC -I=$PROTO_DIR  --csharp_out=$OUT_DIR $PROTO_DIR/dotbpe_option.proto
