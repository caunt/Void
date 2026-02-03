#!/bin/sh
set -e

# Check that required environment variables are set
if [ -z "$CLIENT_CONTAINER_NAME" ]; then
    echo "ERROR: Required environment variables not set:"
    echo "  CLIENT_CONTAINER_NAME=${CLIENT_CONTAINER_NAME}"
    exit 1
fi

# Check that template file exists
if [ ! -f /etc/nginx/templates/default.conf.template ]; then
    echo "ERROR: Template file not found: /etc/nginx/templates/default.conf.template"
    exit 1
fi

# Substitute only our specific environment variables, leaving nginx variables intact
echo "Substituting environment variables in nginx config..."
if ! envsubst '${CLIENT_CONTAINER_NAME}' < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf; then
    echo "ERROR: envsubst failed"
    exit 1
fi

echo "Nginx config generated successfully"
echo "Starting nginx..."

# Start nginx
exec nginx -g 'daemon off;'
