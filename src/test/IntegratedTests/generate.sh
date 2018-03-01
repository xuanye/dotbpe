set -ex

cd $(dirname $0)


unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    windows*)   machine=Windows;;
    *)          machine="UNKNOWN:${unameOut}"
esac


PROTOC=protoc

if [ $machine = "Windows" ] ; then
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link.cmd
elif [ $machine = "Cygwin" ] ; then
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link.cmd
elif [ $machine = "MinGw" ] ; then
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link.cmd
else
  PLUGIN=protoc-gen-dotbpe=dotbpe-amp-link
fi


OUT_DIR=./MathCommon/_g

PROTO_DIR=./protos
BASE_PROTO_DIR=../../protos

if [ -d $OUT_DIR ]; then
  rm -rf $OUT_DIR
fi

mkdir -p $OUT_DIR


$PROTOC -I=$BASE_PROTO_DIR -I=$PROTO_DIR  --csharp_out=$OUT_DIR --dotbpe_out=$OUT_DIR \
	$PROTO_DIR/math.proto --plugin=$PLUGIN