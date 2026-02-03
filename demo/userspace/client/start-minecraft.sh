#!/usr/bin/env bash
set -e

rm -f /tmp/.X0-lock

# 1. Start Display
Xvfb :0 -screen 0 1920x1080x24 &
sleep 1

# 2. Start Window Manager
# -no_cursor prevents Matchbox from drawing a pointer that gets out of sync
matchbox-window-manager -use_titlebar no -use_cursor no &
sleep 1

# 3. Show Splash Screen
wish /opt/splash.tcl &

# 4. THE MAGIC: Pre-configure Minecraft for VNC
# We force Raw Input OFF and drop render distance to 4 to save CPU
mkdir -p /root/.minecraft
cat <<EOF > /root/.minecraft/options.txt
rawMouseInput:false
mouseSensitivity:0
pauseOnLostFocus:false
renderDistance:4
ao:false
particles:0
maxFps:50
touchscreen:true
EOF

# 5. Start x11vnc with ONLY guaranteed options
# -pointer_mode 1 is the most stable for mouse-heavy apps
x11vnc -ncache_cr -display :0 -nopw -forever -shared -rfbport 5900 -bg \
       -pointer_mode 1 -noxfixes -cursor arrow &
sleep 1

# 6. Start Websockify
/opt/websockify/run --web=/opt/novnc 6080 127.0.0.1:5900 &

# 7. Start Minecraft
export LIBGL_ALWAYS_SOFTWARE=1
export MESA_GL_VERSION_OVERRIDE=3.3
export MESA_GLSL_VERSION_OVERRIDE=330

echo "--- Starting Minecraft ---"
exec python3 -m portablemc start --demo release \
    --jvm-args "-XX:+UseG1GC -Djava.awt.headless=false" \
	--server "${SERVER:-proxy}"