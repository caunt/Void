#!/usr/bin/env bash
set -e

rm -f /tmp/.X0-lock

# 1. Start Display
Xvfb :0 -screen 0 1280x720x24 &
sleep 1

# 2. Start Window Manager
# -no_cursor prevents Matchbox from drawing a pointer that gets out of sync
matchbox-window-manager -use_titlebar no -use_cursor no &
sleep 1

# 3. Show Splash Screen
wish /opt/splash.tcl &

# 4. THE MAGIC: Pre-configure Minecraft for VNC
# We force Raw Input OFF and drop render distance to 4 to save CPU
mkdir -p config

cat <<EOF > options.txt
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

cat <<EOF > config/sodium-options.json
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

# 5. Start x11vnc with ONLY guaranteed options
# -pointer_mode 1 is the most stable for mouse-heavy apps
x11vnc -ncache_cr -display :0 -nopw -forever -shared -rfbport 5900 -bg \
       -pointer_mode 1 -noxfixes -cursor arrow &
sleep 1

# 6. Start Websockify
/opt/websockify/run --web=/opt/novnc 80 127.0.0.1:5900 &

# 7. Download Sodium
mkdir -p mods
curl -L -o mods/sodium.jar https://mediafilez.forgecdn.net/files/7527/475/sodium-neoforge-0.8.4%2Bmc1.21.11.jar

# 8. Start Minecraft
export LIBGL_ALWAYS_SOFTWARE=1
export MESA_GL_VERSION_OVERRIDE=3.3
export MESA_GLSL_VERSION_OVERRIDE=330

echo "--- Starting Minecraft ---"
exec /opt/portablemc start neoforge:1.21.11:unstable --demo --mc-dir . --jvm-arg="-Djava.awt.headless=false" $PORTABLEMC_ARGUMENTS