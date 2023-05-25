#!/usr/bin/env bash

REPO=$(cat repo.txt)

echo Pushing to repo: $REPO

docker build -t rc2 -f rc2/Dockerfile .
docker tag rc2:latest $REPO:latest
docker push $REPO:latest
