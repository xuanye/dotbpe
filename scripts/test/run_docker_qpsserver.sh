set -ex

docker run -it -p 6201:6201 --rm --net=host --name qpsserver qpsserver:latest
