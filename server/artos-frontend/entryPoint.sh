#!/bin/sh
MAIN=`find /usr/share/nginx/html/main.*.js`;
sed -i s/API_LINK/$API_LINK/g $MAIN
sed -i s/GIT_VERSION/$(git rev-parse HEAD)/g $MAIN
exec "$@"

