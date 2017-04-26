set -ex

docker run -it -d -p 6201:6201 --name qpsserver qpsserver:latest
