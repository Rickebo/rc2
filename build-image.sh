#!/usr/bin/env bash

docker build -t rc2 -f rc2/Dockerfile .
docker tag rc2:latest repopummcs2xbx.rickebo.com/rickebo/rc2:latest
docker push repopummcs2xbx.rickebo.com/rickebo/rc2:latest
