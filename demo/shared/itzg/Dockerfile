FROM tsl0922/ttyd@sha256:9ac4c4d0b436127af7703402fbd7ae51ff49179ca06ad0a132d7c5cc157015a7 AS ttyd_source

FROM debian:bookworm-slim@sha256:4724b8cc51e33e398f0e2e15e18d5ec2851ff0c2280647e1310bc1642182655d AS tmux_source
RUN apt-get update && apt-get install -y --no-install-recommends tmux && rm -rf /var/lib/apt/lists/*
RUN set -e; \
  mkdir -p /out/usr/local/tmux/bin /out/usr/local/tmux/lib; \
  cp /usr/bin/tmux /out/usr/local/tmux/bin/; \
  ldd /usr/bin/tmux | awk '$3 ~ /^\// {print $3}' | grep -vE '/ld-linux|/libc\.so' | while read -r libraryPath; do cp "$libraryPath" /out/usr/local/tmux/lib/; done

FROM itzg/minecraft-server@sha256:de8f8a7d0f061784616168f84184bb6f1f60d65e98a184d9c9cd04cd41cd50a1

COPY --from=ttyd_source /usr/bin/ttyd /usr/local/bin/ttyd
COPY --from=tmux_source /out/usr/local/tmux /usr/local/tmux

COPY <<'EOF' /entrypoint.sh
#!/bin/sh
set -e

# Point to our custom tmux libs
export LD_LIBRARY_PATH=/usr/local/tmux/lib

# Create the session (detached)
/usr/local/tmux/bin/tmux new-session -d -s main -n shell sh -lc 'unset LD_LIBRARY_PATH; exec /start' || true

# Set options
/usr/local/tmux/bin/tmux set-option -g -w aggressive-resize off
/usr/local/tmux/bin/tmux set-option -g -w window-size largest

# Start ttyd
exec /usr/local/bin/ttyd -R -p 80 sh -lc 'exec env LD_LIBRARY_PATH=/usr/local/tmux/lib /usr/local/tmux/bin/tmux attach -t main'
EOF

RUN sed -i 's/\r$//' /entrypoint.sh && chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
