using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public class PortableMinecraftClient : IIntegrationSide
{
    private const string DockerHost = "host.docker.internal";

    private readonly string _workingDirectory;
    private readonly IFutureDockerImage _image;
    private readonly List<string> _logs = [];
    private readonly Lock _logsLock = new();

    public static TheoryData<ProtocolVersion> SupportedVersions { get; } = [.. ProtocolVersion.Range()];

    public IEnumerable<string> Logs
    {
        get
        {
            lock (_logsLock)
                return [.. _logs];
        }
    }

    private PortableMinecraftClient(string workingDirectory, IFutureDockerImage image)
    {
        _workingDirectory = workingDirectory;
        _image = image;
    }

    public static async Task<PortableMinecraftClient> CreateAsync(string workingDirectory, CancellationToken cancellationToken = default)
    {
        workingDirectory = Path.Combine(workingDirectory, nameof(PortableMinecraftClient));

        if (!Directory.Exists(workingDirectory))
            Directory.CreateDirectory(workingDirectory);

        var dockerfilePath = Path.Combine(workingDirectory, "Dockerfile");

        await File.WriteAllTextAsync(dockerfilePath,
            """
            FROM rust:bookworm AS builder
            
            RUN cargo install portablemc-cli
            
            FROM debian:bookworm-slim
            
            ENV DEBIAN_FRONTEND=noninteractive
            ENV DISPLAY=:99
            ENV LIBGL_ALWAYS_SOFTWARE=1
            ENV MESA_GL_VERSION_OVERRIDE=3.3
            ENV MESA_GLSL_VERSION_OVERRIDE=330
            
            COPY --from=builder /usr/local/cargo/bin/portablemc /usr/local/bin/portablemc
            
            RUN apt-get update && apt-get install -y \
                xvfb \
                xfwm4 \
                x11-utils \
                xdotool \
                libasound2 \
                libflite1 \
                libgl1-mesa-dri \
                libxcursor1 \
                libxrandr2 \
                libxi6 \
                libxtst6 \
                libasound2 \
                libfreetype6 \
                libfontconfig1 \
                ca-certificates \
             && rm -rf /var/lib/apt/lists/*
            
            RUN printf "pcm.!default { type null }\nctl.!default { type null }\n" > /etc/asound.conf
            
            RUN printf '%s\n' \
            '#!/usr/bin/env bash' \
            'set -e' \
            'export DISPLAY="${DISPLAY:-:99}"' \
            'windowId="$(xdotool search --onlyvisible --name "Minecraft" | head -n 1)"' \
            'xdotool windowfocus "$windowId"' \
            'xdotool key --window "$windowId" t' \
            'sleep 1' \
            'xdotool type --window "$windowId" --delay 1 -- "$*"' \
            'xdotool key --window "$windowId" Return' \
            > /usr/local/bin/send-chat \
             && chmod +x /usr/local/bin/send-chat
            
            RUN printf '%s\n' \
            '#!/usr/bin/env bash' \
            'set -e' \
            'mkdir -p "$HOME/.portablemc"' \
            'touch "$HOME/.portablemc/options.txt"' \
            'if grep -q "^onboardAccessibility:" "$HOME/.portablemc/options.txt"; then' \
            '  sed -i "s/^onboardAccessibility:.*/onboardAccessibility:false/" "$HOME/.portablemc/options.txt"' \
            'else' \
            '  printf "onboardAccessibility:false\n" >> "$HOME/.portablemc/options.txt"' \
            'fi' \
            'Xvfb :99 -screen 0 1280x720x24 &' \
            'sleep 2' \
            'xfwm4 &' \
            'sleep 1' \
            'exec portablemc --main-dir "$HOME/.portablemc" "$@"' \
            > /entrypoint.sh \
             && chmod +x /entrypoint.sh
            
            ENTRYPOINT ["/entrypoint.sh"]
            CMD ["release"]
            """, cancellationToken);

        var imageBuilder = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(workingDirectory)
            .WithDockerfile("Dockerfile");

        if (RuntimeInformation.OSArchitecture is not Architecture.X64 and not Architecture.X86)
            imageBuilder = imageBuilder.WithCreateParameterModifier(parameters => parameters.Platform = "linux/amd64");

        var image = imageBuilder.Build();

        await image.CreateAsync(cancellationToken);

        return new PortableMinecraftClient(workingDirectory, image);
    }

    public Task SendTextMessageAsync(EndPoint endPoint, ProtocolVersion protocolVersion, string text, CancellationToken cancellationToken = default)
    {
        return SendTextMessagesAsync(endPoint, protocolVersion, [text], cancellationToken);
    }

    public async Task SendTextMessagesAsync(EndPoint endPoint, ProtocolVersion protocolVersion, IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        lock (_logsLock)
            _logs.Clear();

        var portableMcWorkingDirectory = Directory.CreateDirectory(Path.Combine(_workingDirectory, ".portablemc"));

        (string host, int port) = endPoint switch
        {
            DnsEndPoint dnsEndPoint when !OperatingSystem.IsLinux() && string.Equals(dnsEndPoint.Host, "localhost", StringComparison.OrdinalIgnoreCase) => (DockerHost, dnsEndPoint.Port),
            DnsEndPoint dnsEndPoint => (dnsEndPoint.Host, dnsEndPoint.Port),
            IPEndPoint ipEndPoint when !OperatingSystem.IsLinux() && IPAddress.IsLoopback(ipEndPoint.Address) => (DockerHost, ipEndPoint.Port),
            IPEndPoint ipEndPoint => (ipEndPoint.Address.ToString(), ipEndPoint.Port),
            _ => throw new NotSupportedException($"Unsupported endpoint type: {endPoint.GetType()}")
        };

        await using var logConsumer = new LogConsumer(_logs, _logsLock);

        var containerBuilder = new ContainerBuilder(_image)
            .WithOutputConsumer(logConsumer)
            .WithBindMount(portableMcWorkingDirectory.FullName, "/root/.portablemc")
            .WithCommand(
                "start",
                "--username", nameof(PortableMinecraftClient)[..16],
                "--join-server", host,
                "--join-server-port", port.ToString(),
                "--jvm-arg=-Djava.awt.headless=false",
                protocolVersion.VersionIntroducedIn);

        if (RuntimeInformation.OSArchitecture is not Architecture.X64 and not Architecture.X86)
            containerBuilder = containerBuilder.WithCreateParameterModifier(parameters => parameters.Platform = "linux/amd64");

        if (OperatingSystem.IsLinux())
            containerBuilder = containerBuilder.WithCreateParameterModifier(parameters => parameters.HostConfig.NetworkMode = "host");

        var container = containerBuilder.Build();

        try
        {
            await container.StartAsync(cancellationToken);

            await logConsumer.WaitForTextAsync("Connecting to", cancellationToken);

            // Wait for silence in the logs
            while (logConsumer.TimeSinceLastLog <= TimeSpan.FromSeconds(10))
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            foreach (var text in texts)
            {
                await container.ExecAsync(["send-chat", text], cancellationToken);
                await Task.Delay(3_000, cancellationToken);
            }
        }
        finally
        {
            await container.StopAsync(CancellationToken.None);
            await container.DisposeAsync();
        }
    }

    public void ClearLogs()
    {
        lock (_logsLock)
            _logs.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _image.DeleteAsync();
        await _image.DisposeAsync();
    }

    private sealed class LogConsumer : IOutputConsumer, IAsyncDisposable
    {
        private readonly List<string> _logs;
        private readonly Lock _logsLock;
        private readonly Pipe _stdoutPipe = new();
        private readonly Pipe _stderrPipe = new();
        private readonly Task _stdoutTask;
        private readonly Task _stderrTask;
        private DateTimeOffset _lastLogTimestamp = DateTimeOffset.UtcNow;

        public bool Enabled => true;
        public Stream Stdout => _stdoutPipe.Writer.AsStream();
        public Stream Stderr => _stderrPipe.Writer.AsStream();
        public TimeSpan TimeSinceLastLog
        {
            get
            {
                lock (_logsLock)
                    return DateTimeOffset.UtcNow - _lastLogTimestamp;
            }
        }

        public LogConsumer(List<string> logs, Lock logsLock)
        {
            _logs = logs;
            _logsLock = logsLock;
            _stdoutTask = ReadLinesAsync(_stdoutPipe.Reader);
            _stderrTask = ReadLinesAsync(_stderrPipe.Reader);
        }

        public async Task WaitForTextAsync(string text, CancellationToken cancellationToken)
        {
            var checkedUpTo = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int newCount;
                var found = false;

                lock (_logsLock)
                {
                    newCount = _logs.Count;

                    for (var i = checkedUpTo; i < newCount; i++)
                    {
                        if (_logs[i].Contains(text, StringComparison.Ordinal))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                    return;

                checkedUpTo = newCount;
                await Task.Delay(100, cancellationToken);
            }
        }

        private async Task ReadLinesAsync(PipeReader reader)
        {
            var decoder = Encoding.UTF8.GetDecoder();
            var lineBuilder = new StringBuilder();

            try
            {
                while (true)
                {
                    var result = await reader.ReadAsync();
                    var buffer = result.Buffer;

                    foreach (var segment in buffer)
                    {
                        var bytes = segment.Span;
                        var charCount = decoder.GetCharCount(bytes, flush: false);
                        var chars = charCount > 0 ? new char[charCount] : [];
                        var charsDecoded = decoder.GetChars(bytes, chars, flush: false);

                        for (var i = 0; i < charsDecoded; i++)
                        {
                            var ch = chars[i];

                            if (ch == '\n')
                            {
                                var line = lineBuilder.ToString().TrimEnd('\r');
                                lineBuilder.Clear();

                                if (!string.IsNullOrEmpty(line))
                                {
                                    lock (_logsLock)
                                    {
                                        _logs.Add(line);
                                        _lastLogTimestamp = DateTimeOffset.UtcNow;
                                    }
                                }
                            }
                            else
                            {
                                lineBuilder.Append(ch);
                            }
                        }
                    }

                    reader.AdvanceTo(buffer.End);

                    if (result.IsCompleted)
                    {
                        var flushCount = decoder.GetCharCount(ReadOnlySpan<byte>.Empty, flush: true);

                        if (flushCount > 0)
                        {
                            var flushChars = new char[flushCount];
                            decoder.GetChars(ReadOnlySpan<byte>.Empty, flushChars, flush: true);
                            lineBuilder.Append(flushChars);
                        }

                        if (lineBuilder.Length > 0)
                        {
                            var line = lineBuilder.ToString().TrimEnd('\r');

                            if (!string.IsNullOrEmpty(line))
                            {
                                lock (_logsLock)
                                {
                                    _logs.Add(line);
                                    _lastLogTimestamp = DateTimeOffset.UtcNow;
                                }
                            }
                        }

                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the pipe is completed during cancellation
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"[LogConsumer] Unexpected error reading log stream: {exception}");
            }
            finally
            {
                await reader.CompleteAsync();
            }
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            _stdoutPipe.Writer.Complete();
            _stderrPipe.Writer.Complete();

            await _stdoutTask;
            await _stderrTask;
        }
    }
}
