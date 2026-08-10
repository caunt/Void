using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Connections;

public class RawConnectionTests : IntegrationUnitBase
{
    private const string ExpectedResponse = "Hello through Void!";

    [Fact]
    public async Task HttpClientConnectsToHttpServerThroughProxyAsync()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));

        await using var httpServer = builder.Build();
        httpServer.MapGet("/raw-proxy", () => ExpectedResponse);
        await httpServer.StartAsync(Timeouts.SetupTimeoutToken);

        var httpServerAddress = new Uri(Assert.Single(httpServer.Urls));

        await using var voidProxy = await VoidProxy.CreateAsync(
            Path.Combine(Path.GetTempPath(), nameof(RawConnectionTests), Path.GetRandomFileName()),
            httpServerAddress.Authority,
            cancellationToken: Timeouts.SetupTimeoutToken);

        await LoggedExecutorAsync(async () =>
        {
            var requestStartedAt = DateTime.UtcNow;
            var playerDisconnectedTask = voidProxy.LogWriter.WaitForLineAsync(
                line => line.Contains(" disconnected", StringComparison.Ordinal),
                Timeouts.StepTimeoutToken);

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://{IPAddress.Loopback}:{voidProxy.Port}")
            };
            using var request = new HttpRequestMessage(HttpMethod.Get, "/raw-proxy");
            request.Headers.ConnectionClose = true;
            using var response = await httpClient.SendAsync(request, Timeouts.StepTimeoutToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ExpectedResponse, await response.Content.ReadAsStringAsync(Timeouts.StepTimeoutToken));

            await playerDisconnectedTask;

            var requestLogs = voidProxy.LogWriter.GetLinesSince(requestStartedAt);
            Assert.Contains(requestLogs, line => line.Contains("Channel builder not found", StringComparison.Ordinal));
            Assert.DoesNotContain(requestLogs, line =>
                line.Contains(" ERR] ", StringComparison.Ordinal) ||
                line.Contains(" FTL] ", StringComparison.Ordinal));
        }, voidProxy);
    }

    [Fact]
    public async Task UnsupportedMinecraftProtocolPacketsAreForwardedRawAsync()
    {
        using var fakeServerListener = new TcpListener(IPAddress.Loopback, 0);
        fakeServerListener.Start();

        var fakeServerEndPoint = Assert.IsType<IPEndPoint>(fakeServerListener.LocalEndpoint);

        await using var voidProxy = await VoidProxy.CreateAsync(
            Path.Combine(Path.GetTempPath(), nameof(RawConnectionTests), Path.GetRandomFileName()),
            fakeServerEndPoint.ToString(),
            cancellationToken: Timeouts.SetupTimeoutToken);

        await LoggedExecutorAsync(async () =>
        {
            var connectionStartedAt = DateTime.UtcNow;
            var playerDisconnectedTask = voidProxy.LogWriter.WaitForLineAsync(
                line => line.Contains(" disconnected", StringComparison.Ordinal),
                Timeouts.StepTimeoutToken);

            var unsupportedProtocolVersion = checked(ProtocolVersion.Latest.Value + 1);
            var handshakePacket = CreateHandshakePacket(unsupportedProtocolVersion, IPAddress.Loopback.ToString(), checked((ushort)voidProxy.Port));
            byte[] statusRequestPacket = [0x01, 0x00];
            byte[] statusResponsePacket = [0x04, 0x00, 0x02, 0x7B, 0x7D];
            byte[] pingPacket = [0x09, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2A];

            Assert.True(unsupportedProtocolVersion > ProtocolVersion.Latest.Value);

            var fakeServerAcceptTask = fakeServerListener.AcceptTcpClientAsync(Timeouts.StepTimeoutToken);
            using var fakeClient = new TcpClient(AddressFamily.InterNetwork);
            await fakeClient.ConnectAsync(IPAddress.Loopback, voidProxy.Port, Timeouts.StepTimeoutToken);

            using var fakeClientStream = fakeClient.GetStream();
            await fakeClientStream.WriteAsync(handshakePacket, Timeouts.StepTimeoutToken);

            using var fakeServer = await fakeServerAcceptTask;
            using var fakeServerStream = fakeServer.GetStream();
            var forwardedHandshakePacket = new byte[handshakePacket.Length];
            await fakeServerStream.ReadExactlyAsync(forwardedHandshakePacket, Timeouts.StepTimeoutToken);
            Assert.Equal(handshakePacket, forwardedHandshakePacket);

            await AssertPacketForwardedAsync(fakeClientStream, fakeServerStream, statusRequestPacket, Timeouts.StepTimeoutToken);
            await AssertPacketForwardedAsync(fakeServerStream, fakeClientStream, statusResponsePacket, Timeouts.StepTimeoutToken);
            await AssertPacketForwardedAsync(fakeClientStream, fakeServerStream, pingPacket, Timeouts.StepTimeoutToken);
            await AssertPacketForwardedAsync(fakeServerStream, fakeClientStream, pingPacket, Timeouts.StepTimeoutToken);

            fakeClient.Close();
            await playerDisconnectedTask;

            var connectionLogs = voidProxy.LogWriter.GetLinesSince(connectionStartedAt);
            Assert.Contains(connectionLogs, line => line.Contains("Channel builder not found", StringComparison.Ordinal));
            Assert.DoesNotContain(connectionLogs, line =>
                line.Contains(" ERR] ", StringComparison.Ordinal) ||
                line.Contains(" FTL] ", StringComparison.Ordinal));
        }, voidProxy);
    }

    private static byte[] CreateHandshakePacket(int protocolVersion, string serverAddress, ushort serverPort)
    {
        using var packetDataStream = new MemoryStream();
        var packetDataBuffer = new MinecraftBuffer(packetDataStream);
        packetDataBuffer.WriteVarInt(0);
        packetDataBuffer.WriteVarInt(protocolVersion);
        packetDataBuffer.WriteString(serverAddress);
        packetDataBuffer.WriteUnsignedShort(serverPort);
        packetDataBuffer.WriteVarInt(1);

        var packetData = packetDataStream.ToArray();
        using var packetStream = new MemoryStream();
        var packetBuffer = new MinecraftBuffer(packetStream);
        packetBuffer.WriteVarInt(packetData.Length);
        packetStream.Write(packetData);

        return packetStream.ToArray();
    }

    private static async Task AssertPacketForwardedAsync(NetworkStream sendingStream, NetworkStream receivingStream, byte[] expectedPacket, CancellationToken cancellationToken)
    {
        await sendingStream.WriteAsync(expectedPacket, cancellationToken);

        var forwardedPacket = new byte[expectedPacket.Length];
        await receivingStream.ReadExactlyAsync(forwardedPacket, cancellationToken);

        Assert.Equal(expectedPacket, forwardedPacket);
    }
}
