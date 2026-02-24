⚠️ **Warning!**  

You're not supposed to run this unless you want to run whole Void demo website.  
If you're looking for a examples of how to run Void proxy locally, follow [**this guide**](https://void.caunt.world/docs/).

# Online
[**void-demo.caunt.world**](https://void-demo.caunt.world/)

# Local Docker
## Build
`docker build -t caunt/void-demo:latest .`  
→ [**localhost:8080**](http://localhost:8080/)

## Build
`docker run --rm --privileged -v demo-dind:/var/lib/docker -p 8080:80 -e REDIRECT_LOGS=true caunt/void-demo:latest`  
→ [**localhost:8080**](http://localhost:8080/)

## Cleanup
`docker volume rm demo-dind && docker rmi caunt/void-demo:latest`
