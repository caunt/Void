FROM tsl0922/ttyd AS ttyd_source

FROM debian:bookworm-slim AS tmux_source
RUN apt-get update && apt-get install -y --no-install-recommends tmux && rm -rf /var/lib/apt/lists/*
RUN set -e; \
  mkdir -p /out/usr/local/tmux/bin /out/usr/local/tmux/lib; \
  cp /usr/bin/tmux /out/usr/local/tmux/bin/; \
  ldd /usr/bin/tmux | awk '$3 ~ /^\// {print $3}' | grep -vE '/ld-linux|/libc\.so' | while read -r libraryPath; do cp "$libraryPath" /out/usr/local/tmux/lib/; done

FROM itzg/minecraft-server

COPY --from=ttyd_source /usr/bin/ttyd /usr/local/bin/ttyd
COPY --from=tmux_source /out/usr/local/tmux /usr/local/tmux

Will this work correctly to survive ttyd page refreshes?

ENTRYPOINT ["sh","-lc","set -e; export LD_LIBRARY_PATH=/usr/local/tmux/lib; /usr/local/tmux/bin/tmux start-server; /usr/local/tmux/bin/tmux set-option -g -w aggressive-resize off; /usr/local/tmux/bin/tmux set-option -g -w window-size largest; /usr/local/tmux/bin/tmux has-session -t main 2>/dev/null || /usr/local/tmux/bin/tmux new-session -d -s main sh -lc 'unset LD_LIBRARY_PATH; exec /start'; exec /usr/local/bin/ttyd -R -p 80 sh -lc 'exec env LD_LIBRARY_PATH=/usr/local/tmux/lib /usr/local/tmux/bin/tmux attach -t main'"]
