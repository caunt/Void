# Run
`docker run --rm --privileged -v demo-dind:/var/lib/docker -p 8080:80 -e REDIRECT_LOGS=true caunt/void-demo:latest`

# Run Locally
`docker build -t caunt/void-demo:latest . && docker run --rm --privileged -v demo-dind:/var/lib/docker -p 8080:80 -e REDIRECT_LOGS=true caunt/void-demo:latest`

# Cleanup
`docker volume rm demo-dind && docker rmi caunt/void-demo:latest`