# Builder stage: compile portablemc from source
FROM --platform=linux/amd64 rust:1.96-slim-trixie@sha256:26abcef3d79b8d890c4ceb17093154573e1f6479cf6dd7c1450043b8458350f6 AS builder

RUN apt-get update && apt-get install -y --no-install-recommends \
    pkg-config \
    libssl-dev \
 && rm -rf /var/lib/apt/lists/*

RUN cargo install portablemc-cli --root /usr/local

# Runtime stage
FROM --platform=linux/amd64 amd64/debian:trixie-20260518@sha256:b91d9b2c1738c0cb7f2c7440eaebc9f40e3e411a0e7ce768dd4e1879cf0cd141 AS latest

ENV DEBIAN_FRONTEND=noninteractive
ENV LIBGL_ALWAYS_SOFTWARE=1

# Copy pre-built portablemc binary from builder
COPY --from=builder /usr/local/bin/portablemc /usr/local/bin/portablemc

# Install only runtime dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    jq \
    imagemagick \
    libasound2 \
    libflite1 \
    libfontconfig1 \
    libfreetype6 \
    libgl1-mesa-dri \
    libicu-dev \
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

# Install .NET 11 preview
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh \
 && chmod +x /tmp/dotnet-install.sh \
 && /tmp/dotnet-install.sh --channel 11.0 --quality preview --install-dir /usr/share/dotnet \
 && rm /tmp/dotnet-install.sh

ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="${DOTNET_ROOT}:${PATH}"

# Fix sound errors by using the null audio driver
RUN printf "pcm.!default { type null }\nctl.!default { type null }\n" > /etc/asound.conf

# Skip Minecraft accessibility screen
RUN mkdir -p "$HOME/.minecraft" && touch "$HOME/.minecraft/options.txt"

# Copy the API application
COPY Api.cs /app/Api.cs
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "run", "Api.cs"]

FROM latest AS offline

ARG FIRST_RELEASE_VERSION=1.7.2

# Dry-run all release versions in reverse order (from oldest to latest)
RUN curl -fsSL https://launchermeta.mojang.com/mc/game/version_manifest_v2.json \
  | jq -r --arg first "$FIRST_RELEASE_VERSION" \
    '([.versions[] | select(.type == "release") | .id] | reverse) as $r | ($r | index($first)) as $i | if $i == null then error("Not found: \($first)") else $r[$i:] end | .[]' \
  | while IFS= read -r minecraftVersion; do portablemc start --dry "mojang:$minecraftVersion"; done

FROM latest AS version

ARG MINECRAFT_VERSION
RUN test -n "$MINECRAFT_VERSION"
RUN portablemc start --dry "mojang:$MINECRAFT_VERSION"
