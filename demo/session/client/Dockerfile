# ==========================================
# STAGE 1: Builder
# ==========================================
FROM debian:trixie-slim AS builder
RUN apt-get update && apt-get install -y --no-install-recommends curl unzip ca-certificates
WORKDIR /src
RUN curl -L -o novnc.zip https://github.com/novnc/noVNC/archive/refs/tags/v1.5.0.zip \
    && unzip -q novnc.zip && mv noVNC-1.5.0 novnc && rm novnc.zip
RUN curl -L -o websockify.zip https://github.com/novnc/websockify/archive/refs/tags/v0.12.0.zip \
    && unzip -q websockify.zip && mv websockify-0.12.0 websockify && rm websockify.zip
RUN curl -L -o portablemc.tar.gz https://github.com/mindstorm38/portablemc/releases/download/v5.0.2/portablemc-5.0.2-linux-x86_64-gnu.tar.gz \
    && tar xzvf portablemc.tar.gz && mv portablemc-*/portablemc portablemc && rm portablemc.tar.gz

# ==========================================
# STAGE 2: Runtime
# ==========================================
FROM debian:trixie-slim

ENV DISPLAY=:0 \
    LIBGL_ALWAYS_SOFTWARE=1 \
    PYTHONUNBUFFERED=1 \
    DEBIAN_FRONTEND=noninteractive \
    MESA_GL_VERSION_OVERRIDE=3.3 \
    MESA_GLSL_VERSION_OVERRIDE=330 \
    PYTHONPATH="/opt/websockify:/opt/portablemc"

COPY --from=builder /src/novnc /opt/novnc
COPY --from=builder /src/websockify /opt/websockify
COPY --from=builder /src/portablemc /opt/portablemc
RUN ln -s /opt/novnc/vnc.html /opt/novnc/index.html

# Force noVNC to use 'Relative' pointer mode and better scaling by default
RUN sed -i "s/'view_only', false/'view_only', false/g" /opt/novnc/app/ui.js && \
    sed -i "s/showDotCursor: false/showDotCursor: true/g" /opt/novnc/app/ui.js

# Splash Screen script
RUN echo 'wm attributes . -fullscreen 1; . configure -bg black; label .l -text "Starting Minecraft...\n\n\nThis can take up to 1 minute." -font {Helvetica 30} -fg white -bg black; pack .l -expand 1 -fill both' > /opt/splash.tcl

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    python3 \
    # libpython3.11-stdlib \
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
    binutils
    # # --- SURGERY ---
    # && find /usr/lib/*/dri/ -type f ! -name "swrast_dri.so" -delete \
    # && strip --strip-unneeded /usr/lib/*/libLLVM-15.so.1 \
    # && strip --strip-unneeded /usr/lib/*/dri/swrast_dri.so \
    # # --- REFINED PYTHON PRUNING ---
    # # We kept 'email' this time because http.client needs it!
    # && rm -rf /usr/lib/python3.11/sqlite3 \
    # && rm -rf /usr/lib/python3.11/tkinter \
    # && rm -rf /usr/lib/python3.11/unittest \
    # && rm -rf /usr/lib/python3.11/test \
    # && rm -rf /usr/lib/python3.11/idlelib \
    # && rm -rf /usr/lib/python3.11/ensurepip \
    # && rm -rf /usr/lib/python3.11/pydoc_data \
    # && find /usr/lib/python3.11 -name "__pycache__" -exec rm -rf {} + \
    # # --- CLEANUP ---
    # && apt-get purge -y --auto-remove binutils \
    # && rm -rf /usr/share/doc /usr/share/man /usr/share/info \
    # && rm -rf /var/lib/apt/lists/* /var/cache/apt/*

COPY start.sh /start.sh
RUN sed -i 's/\r$//' /start.sh && chmod +x /start.sh

EXPOSE 80
CMD ["/start.sh"]