using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Extensions;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Void.IntegrationTests.Infrastructure.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class ProxiedAuthenticationTests : IntegrationUnitBase
{
    private const string ExpectedKickReason = "The server is full!";
    private const string Username = "Shonz1";

    [Fact]
    public async Task PaperServerFullKickIsRelayedToPlayerAsync()
    {
        await using var paperServer = await PaperServer.CreateAsync(
            "server-full.log",
            Timeouts.SetupTimeoutToken,
            maximumPlayers: 0);
        await using var voidProxy = await ProcessVoidProxy.CreateAsync(paperServer.Port, Timeouts.SetupTimeoutToken);

        await LoggedExecutorAsync(async () =>
        {
            var connectionStartedAt = DateTime.UtcNow;
            var authenticationFailureTask = voidProxy.LogWriter.WaitForLineAsync(
                line => line.Contains($"Player {Username} cannot authenticate on args-server-1", StringComparison.Ordinal),
                Timeouts.StepTimeoutToken);
            var playerDisconnectedTask = voidProxy.LogWriter.WaitForLineAsync(
                line => line.Contains($"Player {Username} disconnected", StringComparison.Ordinal),
                Timeouts.StepTimeoutToken);

            using var client = new TcpClient(AddressFamily.InterNetwork);
            await client.ConnectAsync(IPAddress.Loopback, voidProxy.Port, Timeouts.StepTimeoutToken);

            using var clientStream = client.GetStream();
            await clientStream.WriteAsync(CreateHandshakePacket(voidProxy.Port), Timeouts.StepTimeoutToken);
            await clientStream.WriteAsync(CreateLoginStartPacket(), Timeouts.StepTimeoutToken);

            var disconnectPacketLength = await ReadVarIntAsync(clientStream, Timeouts.StepTimeoutToken);
            var disconnectPacketData = new byte[disconnectPacketLength];
            await clientStream.ReadExactlyAsync(disconnectPacketData, Timeouts.StepTimeoutToken);

            var disconnectPacket = new MinecraftBuffer(disconnectPacketData);
            Assert.Equal(0, disconnectPacket.ReadVarInt());
            Assert.Equal(ExpectedKickReason, disconnectPacket.ReadComponent(asNbt: false).AsText);

            client.Close();

            var authenticationFailureLine = await authenticationFailureTask;
            Assert.Contains(ExpectedKickReason, authenticationFailureLine, StringComparison.Ordinal);
            await playerDisconnectedTask;

            await paperServer.ExpectTextAsync(ExpectedKickReason, lookupHistory: true, Timeouts.StepTimeoutToken);
            voidProxy.AssertNoWarningOrHigherLogsSince(connectionStartedAt);
        }, voidProxy, paperServer);
    }

    private static byte[] CreateHandshakePacket(int proxyPort)
    {
        using var packetDataStream = new MemoryStream();
        var packetDataBuffer = new MinecraftBuffer(packetDataStream);
        packetDataBuffer.WriteVarInt(0);
        packetDataBuffer.WriteVarInt(ProtocolVersion.MINECRAFT_1_7_2.Value);
        packetDataBuffer.WriteString(IPAddress.Loopback.ToString());
        packetDataBuffer.WriteUnsignedShort(checked((ushort)proxyPort));
        packetDataBuffer.WriteVarInt(2);

        return FramePacket(packetDataStream.ToArray());
    }

    private static byte[] CreateLoginStartPacket()
    {
        using var packetDataStream = new MemoryStream();
        var packetDataBuffer = new MinecraftBuffer(packetDataStream);
        packetDataBuffer.WriteVarInt(0);
        packetDataBuffer.WriteString(Username);

        return FramePacket(packetDataStream.ToArray());
    }

    private static byte[] FramePacket(byte[] packetData)
    {
        using var packetStream = new MemoryStream();
        var packetBuffer = new MinecraftBuffer(packetStream);
        packetBuffer.WriteVarInt(packetData.Length);
        packetStream.Write(packetData);

        return packetStream.ToArray();
    }

    private static async Task<int> ReadVarIntAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        var value = 0;
        var buffer = new byte[1];

        for (var position = 0; position < 5; position++)
        {
            await stream.ReadExactlyAsync(buffer, cancellationToken);

            value |= (buffer[0] & 0x7F) << (position * 7);

            if ((buffer[0] & 0x80) == 0)
                return value;
        }

        throw new InvalidDataException("VarInt is too big.");
    }

    private sealed record ProcessVoidProxy(Process Process, CollectingTextWriter LogWriter, Task StandardOutputTask, Task StandardErrorTask, int Port) : IIntegrationSide
    {
        public string LogFileName => "void-proxy.log";
        public IEnumerable<string> Logs => LogWriter.Lines;

        public static async Task<ProcessVoidProxy> CreateAsync(int paperServerPort, CancellationToken cancellationToken)
        {
            var workingDirectory = Path.Combine(Path.GetTempPath(), nameof(ProxiedAuthenticationTests), Path.GetRandomFileName());
            Directory.CreateDirectory(workingDirectory);

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            startInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "Void.Proxy.dll"));
            startInfo.ArgumentList.Add("--port");
            startInfo.ArgumentList.Add("0");
            startInfo.ArgumentList.Add("--logging");
            startInfo.ArgumentList.Add("Trace");
            startInfo.ArgumentList.Add("--server");
            startInfo.ArgumentList.Add($"localhost:{paperServerPort}");
            startInfo.ArgumentList.Add("--ignore-file-servers");
            startInfo.ArgumentList.Add("--offline");
            startInfo.ArgumentList.Add("--read-only");

            var process = new Process { StartInfo = startInfo };

            if (!process.Start())
                throw new InvalidOperationException("Failed to start Void proxy process.");

            var logWriter = new CollectingTextWriter();
            var standardOutputTask = CollectOutputAsync(process.StandardOutput, logWriter);
            var standardErrorTask = CollectOutputAsync(process.StandardError, logWriter);
            var proxy = new ProcessVoidProxy(process, logWriter, standardOutputTask, standardErrorTask, Port: 0);

            try
            {
                while (!logWriter.Lines.Any(line => line.Contains("Proxy started", StringComparison.Ordinal)))
                {
                    if (process.HasExited)
                        throw new InvalidOperationException($"Void proxy exited with code {process.ExitCode}. Logs:\n{logWriter.Text}");

                    await Task.Delay(100, cancellationToken);
                }

                var listenerStartedLine = logWriter.Lines.Last(line => line.Contains("Connection listener started on address", StringComparison.Ordinal));
                var portText = listenerStartedLine[(listenerStartedLine.LastIndexOf(':') + 1)..];

                if (!int.TryParse(portText, out var port))
                    throw new InvalidDataException($"Failed to read Void proxy port from log line: {listenerStartedLine}");

                return proxy with { Port = port };
            }
            catch
            {
                await proxy.DisposeAsync();
                throw;
            }
        }

        public void AssertNoWarningOrHigherLogsSince(DateTime since)
        {
            var unexpectedLogs = LogWriter.GetLinesSince(since).Where(line =>
                line.Contains(" WRN] ", StringComparison.Ordinal) ||
                line.Contains(" ERR] ", StringComparison.Ordinal) ||
                line.Contains(" FTL] ", StringComparison.Ordinal)).ToArray();

            if (unexpectedLogs.Length > 0)
                Assert.Fail($"Void emitted warning or higher logs:\n{string.Join('\n', unexpectedLogs)}");
        }

        public void ClearLogs()
        {
            LogWriter.Clear();
        }

        public Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<IEnumerable<string>>(LogWriter.GetLinesSince(since));
        }

        public async ValueTask DisposeAsync()
        {
            await Process.ExitAsync(cancellationToken: Timeouts.StepTimeoutToken);
            await Task.WhenAll(StandardOutputTask, StandardErrorTask).WaitAsync(Timeouts.StepTimeoutToken);
            Process.Dispose();

            GC.SuppressFinalize(this);
        }

        private static async Task CollectOutputAsync(StreamReader reader, TextWriter writer)
        {
            while (await reader.ReadLineAsync() is { } line)
                await writer.WriteLineAsync(line);
        }

    }
}
