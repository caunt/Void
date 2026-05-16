# Online
[**void-demo.caunt.world**](https://void-demo.caunt.world/)

⚠️ **Warning!**  

You're not supposed to run commands below unless you want to run whole Void demo website.  
If you're looking for a examples of how to run Void proxy locally, follow [**this guide**](https://void.caunt.world/docs/).

# Local Docker
## Build
`docker build -t caunt/void-demo:latest .`  

## Run
`docker run --name void-demo --rm --privileged -v demo-dind:/var/lib/docker -p 8080:80 -e REDIRECT_LOGS=true caunt/void-demo:latest`  
→ [**localhost:8080**](http://localhost:8080/)

## Cleanup
`docker rm -f void-demo && docker volume rm demo-dind && docker rmi caunt/void-demo:latest`

# Testing
`docker volume rm demo-dind || true && docker rm -f void-demo || true && docker build -t caunt/void-demo:latest . && docker run --name void-demo --rm --privileged -v demo-dind:/var/lib/docker -p 8080:80 -e REDIRECT_LOGS=true caunt/void-demo:latest`  
→ [**localhost:8080**](http://localhost:8080/)
