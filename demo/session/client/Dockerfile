FROM rust:1.95-slim-trixie@sha256:a6bc7ce698c8c1025307bd85254b697c58c466f66e83012826a1a50937142cbe AS builder

WORKDIR /src

RUN apt-get update && apt-get install -y --no-install-recommends curl ca-certificates pkg-config libssl-dev
RUN cargo install portablemc-cli --root /src

FROM debian:trixie-slim@sha256:109e2c65005bf160609e4ba6acf7783752f8502ad218e298253428690b9eaa4b

ENV DISPLAY=:0 LIBGL_ALWAYS_SOFTWARE=1 DEBIAN_FRONTEND=noninteractive
COPY --from=builder /src/bin/portablemc /usr/local/bin/portablemc

# Splash Screen script
RUN echo 'wm attributes . -fullscreen 1; . configure -bg black; label .l -text "Starting Minecraft...\n\n\nThis can take up to 1 minute." -font {Helvetica 30} -fg white -bg black; pack .l -expand 1 -fill both' > /opt/splash.tcl

RUN apt-get update && apt-get install -y --no-install-recommends \
    jq \
    curl \
    python3 \
    novnc \
    websockify \
    xvfb \
    x11vnc \
    matchbox-window-manager \
    libgl1-mesa-dri \
    libxcursor1 \
    libxrandr2 \
    libxi6 \
    libxtst6 \
    libasound2 \
    libfreetype6 \
    libfontconfig1 \
    ca-certificates \
    binutils \
    xxd \
    && rm -rf /var/lib/apt/lists/*

RUN ln -s /usr/share/novnc /opt/novnc && ln -s /opt/novnc/vnc.html /opt/novnc/index.html

RUN portablemc start neoforge:release:unstable --dry --demo --mc-dir .

# Create a server list with one visible "void" entry.
RUN printf '%s' '0a0000090007736572766572730a0000000101000668696464656e000800026970000a766f69643a32353536350800046e616d65000a566f69642050726f78790000' | xxd -r -p > servers.dat

RUN mkdir -p mods && sh -c 'url="$(curl -fsSL "https://api.modrinth.com/v2/project/AANobbMI/version?loaders=%5B%22neoforge%22%5D&game_versions=%5B%22$(curl -fsSL https://piston-meta.mojang.com/mc/game/version_manifest_v2.json | jq -r ".latest.release")%22%5D" | jq -r "first(.[0].files[]? | select(.primary == true).url) // empty")"; [ -n "$url" ] && curl -fL -o mods/sodium.jar "$url" || echo "Warning: Sodium jar not found for latest NeoForge Minecraft" >&2'

COPY <<'EOF' options.txt
ao:false
cloudRange:128
cutoutLeaves:false
enableVsync:false
entityDistanceScaling:0.5
entityShadows:false
gamma:1.0
graphicsPreset:"custom"
joinedFirstServer:true
maxAnisotropyBit:2
maxFps:30
mipmapLevels:0
mouseSensitivity:0.0
narrator:0
particles:2
pauseOnLostFocus:false
prioritizeChunkUpdates:0
rawMouseInput:false
renderClouds:"fast"
renderDistance:8
simulationDistance:5
syncChunkWrites:false
textureFiltering:0
touchscreen:true
tutorialStep:find_tree
vignette:false
EOF

COPY <<'EOF' config/sodium-options.json
{
  "advanced": {
    "enable_memory_tracing": false,
    "use_advanced_staging_buffers": true,
    "cpu_render_ahead_limit": 3
  },
  "performance": {
    "chunk_builder_threads": 0,
    "chunk_build_defer_mode": "ALWAYS",
    "animate_only_visible_textures": true,
    "use_entity_culling": true,
    "use_fog_occlusion": true,
    "use_block_face_culling": true,
    "use_no_error_g_l_context": true,
    "quad_splitting_mode": "SAFE"
  },
  "notifications": {
    "has_cleared_donation_button": false,
    "has_seen_donation_prompt": false
  },
  "debug": {
    "terrain_sorting_enabled": true
  }
}
EOF

COPY <<'EOF' /entrypoint.sh
#!/usr/bin/env bash
set -e

rm -f /tmp/.X0-lock

# Start Display
Xvfb :0 -screen 0 1280x720x24 &
sleep 1

# Start Window Manager
# -no_cursor prevents Matchbox from drawing a pointer that gets out of sync
matchbox-window-manager -use_titlebar no -use_cursor no &
sleep 1

# Show Splash Screen
wish /opt/splash.tcl &

# Start x11vnc with ONLY guaranteed options
# -pointer_mode 1 is the most stable for mouse-heavy apps
x11vnc -ncache_cr -display :0 -nopw -forever -shared -rfbport 5900 -bg \
       -pointer_mode 1 -noxfixes -cursor arrow &
sleep 1

# Start Websockify
websockify --web=/opt/novnc 80 127.0.0.1:5900 &

# Start Minecraft
echo "--- Starting Minecraft ---"
exec portablemc start neoforge:release:unstable --demo --mc-dir . --jvm-arg="-Djava.awt.headless=false" $PORTABLEMC_ARGUMENTS
EOF

EXPOSE 80

RUN sed -i 's/\r$//' /entrypoint.sh && chmod +x /entrypoint.sh
CMD ["/entrypoint.sh"]
