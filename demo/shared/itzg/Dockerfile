FROM tsl0922/ttyd@sha256:9ac4c4d0b436127af7703402fbd7ae51ff49179ca06ad0a132d7c5cc157015a7 AS ttyd_source

FROM debian:bookworm-slim@sha256:abd67ffcfa541b485a3dff59865ab629aa048a6c613e639d36e7456b0b229241 AS tmux_source
RUN apt-get update && apt-get install -y --no-install-recommends tmux && rm -rf /var/lib/apt/lists/*
RUN set -e; \
  mkdir -p /out/usr/local/tmux/bin /out/usr/local/tmux/lib; \
  cp /usr/bin/tmux /out/usr/local/tmux/bin/; \
  ldd /usr/bin/tmux | awk '$3 ~ /^\// {print $3}' | grep -vE '/ld-linux|/libc\.so|/libm\.so|/libpthread\.so|/librt\.so|/libdl\.so|/libresolv\.so|/libnsl\.so|/libutil\.so' | while read -r libraryPath; do cp "$libraryPath" /out/usr/local/tmux/lib/; done

FROM itzg/minecraft-server@sha256:a4594a30320ff9273a759dee6865d59ac04141ff59e72918db90906e887fc581

COPY --from=ttyd_source /usr/bin/ttyd /usr/local/bin/ttyd
COPY --from=tmux_source /out/usr/local/tmux /usr/local/tmux

COPY <<'EOF' /entrypoint.sh
#!/bin/sh
set -e

tmux() {
  env LD_LIBRARY_PATH=/usr/local/tmux/lib /usr/local/tmux/bin/tmux "$@"
}

# Create the session (detached)
tmux new-session -d -s main -n shell sh -lc 'unset LD_LIBRARY_PATH; exec /start' || true

# Set options
tmux set-option -g -w aggressive-resize off
tmux set-option -g -w window-size largest

# Start ttyd
exec /usr/local/bin/ttyd -R -p 80 env LD_LIBRARY_PATH=/usr/local/tmux/lib /usr/local/tmux/bin/tmux attach -t main
EOF

RUN sed -i 's/\r$//' /entrypoint.sh && chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
