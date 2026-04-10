FROM --platform=linux/amd64 amd64/debian:bookworm

ARG FIRST_RELEASE_VERSION=1.7.2

ENV DEBIAN_FRONTEND=noninteractive
ENV LIBGL_ALWAYS_SOFTWARE=1
ENV PATH=/root/.cargo/bin:$PATH

# Install dependencies
RUN apt-get update && apt-get install -y \
    ca-certificates \
    curl \
    jq \
    build-essential \
    pkg-config \
    libssl-dev \
    imagemagick \
    libasound2 \
    libflite1 \
    libfontconfig1 \
    libfreetype6 \
    libgl1-mesa-dri \
    libxcursor1 \
    libxi6 \
    libxrandr2 \
    libxtst6 \
    x11-utils \
    x11-xserver-utils \
    xdotool \
    xvfb \
    xfwm4 \
 && rm -rf /var/lib/apt/lists/*
 
# Install Rust
RUN curl https://sh.rustup.rs -sSf | sh -s -- -y --profile minimal

# Install portablemc
RUN cargo install portablemc-cli

# Dry-run all release versions in reverse order (from oldest to latest)
RUN curl -s https://launchermeta.mojang.com/mc/game/version_manifest_v2.json | jq -r '[.versions[] | select(.type == "release") | .id] | reverse[]' | grep -A99999 "^$FIRST_RELEASE_VERSION$" | while IFS= read -r versionId; do portablemc start --dry "mojang:$versionId"; done

# Fix sound errors by using the null audio driver
RUN printf "pcm.!default { type null }\nctl.!default { type null }\n" > /etc/asound.conf

# Skip Minecraft accessibility screen
RUN mkdir -p "$HOME/.minecraft" && touch "$HOME/.minecraft/options.txt"

# Start display utility
COPY start-display /usr/local/bin/start-display
RUN chmod +x /usr/local/bin/start-display

# Send chat message utility
COPY send-chat /usr/local/bin/send-chat
RUN chmod +x /usr/local/bin/send-chat

ENTRYPOINT ["sleep"]
CMD ["infinity"]
