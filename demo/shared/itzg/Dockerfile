FROM tsl0922/ttyd:latest AS ttyd_source

FROM itzg/minecraft-server

COPY --from=ttyd_source /usr/bin/ttyd /usr/local/bin/ttyd

ENTRYPOINT ["/usr/local/bin/ttyd"]
CMD ["-R","-p","80","/start"]
