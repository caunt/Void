# Builder stage: compile portablemc from source
FROM rust:1.97-slim-trixie@sha256:3b2879047d42784ca9403ad20c51ed3df361a50f1df96f5777d39b4e33aa65cd AS builder

RUN apt-get update && apt-get install -y --no-install-recommends \
    pkg-config \
    libssl-dev \
 && rm -rf /var/lib/apt/lists/*

RUN cargo install portablemc-cli --root /usr/local

# Runtime stage
FROM debian:13@sha256:34cd9e9fd437c0a095ec39cb2e73422c9f30821b0d0848ed74fd0d43bae4d958 AS latest

ARG TARGETARCH

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
    matchbox-window-manager \
    novnc \
    tk \
    websockify \
    x11-utils \
    x11-xserver-utils \
    x11vnc \
    xdotool \
    xxd \
    xvfb \
    xfwm4 \
 && rm -rf /var/lib/apt/lists/*

# Mojang does not publish ARM64 JVMs for every Java version required by older clients.
RUN if [ "$TARGETARCH" = "arm64" ]; then \
      dpkg --add-architecture i386; \
      apt-get update; \
      apt-get install -y --no-install-recommends \
        libc6:i386 \
        libasound2:i386 \
        libfontconfig1:i386 \
        libfreetype6:i386 \
        libgl1:i386 \
        libgl1-mesa-dri:i386 \
        libstdc++6:i386 \
        libxcursor1:i386 \
        libxi6:i386 \
        libxrandr2:i386 \
        libxtst6:i386 \
        qemu-user; \
      rm -rf /var/lib/apt/lists/*; \
      for javaVersion in 8 16 17 21 25; do \
        downloadUrl="$(curl --fail --silent --get 'https://api.azul.com/metadata/v1/zulu/packages/' \
          --data-urlencode "java_version=$javaVersion" \
          --data-urlencode 'os=linux' \
          --data-urlencode 'arch=arm' \
          --data-urlencode 'archive_type=tar.gz' \
          --data-urlencode 'java_package_type=jre' \
          --data-urlencode 'release_status=ga' \
          --data-urlencode 'availability_types=CA' \
          | jq -r '[.[] | select((.name | contains("-linux_aarch64.tar.gz")) and (.name | contains("-fx-") | not))][0].download_url // empty')"; \
        test -n "$downloadUrl"; \
        mkdir -p "/usr/lib/jvm/zulu-$javaVersion"; \
        curl --fail --location --silent --show-error "$downloadUrl" \
          | tar --extract --gzip --directory "/usr/lib/jvm/zulu-$javaVersion" --strip-components=1; \
      done; \
      mkdir -p /opt/zulu-8-i686; \
      curl --fail --location --silent --show-error \
        https://cdn.azul.com/zulu/bin/zulu8.96.0.19-ca-jre8.0.502-linux_i686.tar.gz \
        | tar --extract --gzip --directory /opt/zulu-8-i686 --strip-components=1; \
      mkdir -p /opt/portableminecraftclient; \
      curl --fail --location --silent --show-error \
        https://libraries.minecraft.net/org/lwjgl/lwjgl/3.4.1/lwjgl-3.4.1-unsafe.jar \
        --output /opt/portableminecraftclient/lwjgl-3.4.1-unsafe.jar; \
      curl --fail --location --silent --show-error \
        https://repo.maven.apache.org/maven2/org/lwjgl/lwjgl/3.4.1/lwjgl-3.4.1-natives-linux-arm64.jar \
        --output /opt/portableminecraftclient/lwjgl-3.4.1-natives-linux-arm64.jar; \
    fi

# Install .NET 11 preview
RUN curl --fail --location --silent --show-error --retry 5 --retry-all-errors https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh \
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

COPY list-minecraft-release-versions /usr/local/bin/list-minecraft-release-versions
COPY portablemc-dry-run-with-retries /usr/local/bin/portablemc-dry-run-with-retries
COPY java-i686 /usr/local/bin/java-i686
COPY entrypoint /usr/local/bin/portableminecraftclient-entrypoint
COPY launch-portableminecraftclient /usr/local/bin/launch-portableminecraftclient
COPY options.txt sodium-options.json splash.tcl /opt/portableminecraftclient/
RUN chmod +x /usr/local/bin/list-minecraft-release-versions /usr/local/bin/portablemc-dry-run-with-retries /usr/local/bin/java-i686 /usr/local/bin/portableminecraftclient-entrypoint /usr/local/bin/launch-portableminecraftclient \
 && ln -s /usr/share/novnc/vnc.html /usr/share/novnc/index.html

EXPOSE 80 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV CURSEFORGE_API_BASE_URL=https://api.curseforge.com

ENTRYPOINT ["portableminecraftclient-entrypoint"]

FROM latest AS offline

ARG FIRST_RELEASE_VERSION=1.7.2
ARG LAST_RELEASE_VERSION=26.1.2

# Dry-run all release versions in reverse order (from oldest to latest)
RUN list-minecraft-release-versions "$FIRST_RELEASE_VERSION" "$LAST_RELEASE_VERSION" | while IFS= read -r minecraftVersion; do portablemc-dry-run-with-retries "mojang:$minecraftVersion"; done

FROM latest AS version

ARG MINECRAFT_VERSION
RUN test -n "$MINECRAFT_VERSION"
RUN portablemc-dry-run-with-retries "mojang:$MINECRAFT_VERSION"
