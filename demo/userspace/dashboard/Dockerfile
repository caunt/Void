FROM nginx:alpine

COPY nginx.conf /etc/nginx/templates/default.conf.template
COPY index.html /usr/share/nginx/html/index.html

# nginx:alpine image automatically processes templates in /etc/nginx/templates/
# and substitutes environment variables using envsubst
