#!/bin/sh
set -e

# Substitute only our specific environment variables, leaving nginx variables intact
envsubst '${VOID_CONTAINER_NAME} ${CLIENT_CONTAINER_NAME}' < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf

# Start nginx
exec nginx -g 'daemon off;'
