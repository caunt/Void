using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Network.Streams;
using Void.Proxy.Api.Network.Streams.Manual.Network;
using Xunit;

namespace Void.Tests.NetworkTests;

public class NetworkTests
{
    [Fact]
    public void NetworkChannel_CanRead_WhenConfigured_ReturnsExpectedValue()
    {
        var mockChannel = new Mock<INetworkChannel>();
        mockChannel.Setup(c => c.CanRead).Returns(true);

        var channel = mockChannel.Object;

        Assert.True(channel.CanRead);
    }

    [Fact]
    public void NetworkChannel_CanWrite_WhenConfigured_ReturnsExpectedValue()
    {
        var mockChannel = new Mock<INetworkChannel>();
        mockChannel.Setup(c => c.CanWrite).Returns(true);

        var channel = mockChannel.Object;

        Assert.True(channel.CanWrite);
    }

    [Fact]
    public void NetworkChannel_IsAlive_WhenActive_ReturnsTrue()
    {
        var mockChannel = new Mock<INetworkChannel>();
        mockChannel.Setup(c => c.IsAlive).Returns(true);

        var channel = mockChannel.Object;

        Assert.True(channel.IsAlive);
    }

    [Fact]
    public async Task NetworkChannel_ReadMessageAsync_WhenMessageAvailable_ReturnsMessage()
    {
        var mockChannel = new Mock<INetworkChannel>();
        var mockMessage = new Mock<INetworkMessage>();

        mockChannel.Setup(c => c.ReadMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockMessage.Object);

        var channel = mockChannel.Object;
        var message = await channel.ReadMessageAsync();

        Assert.NotNull(message);
        mockChannel.Verify(c => c.ReadMessageAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NetworkChannel_WriteMessageAsync_WhenCalled_WritesMessage()
    {
        var mockChannel = new Mock<INetworkChannel>();
        var mockMessage = new Mock<INetworkMessage>();

        mockChannel.Setup(c => c.WriteMessageAsync(It.IsAny<INetworkMessage>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var channel = mockChannel.Object;
        await channel.WriteMessageAsync(mockMessage.Object);

        mockChannel.Verify(c => c.WriteMessageAsync(mockMessage.Object, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void NetworkChannel_Pause_WhenCalledWithReadOperation_PausesChannel()
    {
        var mockChannel = new Mock<INetworkChannel>();

        mockChannel.Setup(c => c.Pause(It.IsAny<Operation>()));

        var channel = mockChannel.Object;
        channel.Pause(Operation.Read);

        mockChannel.Verify(c => c.Pause(Operation.Read), Times.Once);
    }

    [Fact]
    public void NetworkChannel_Resume_WhenCalledWithReadOperation_ResumesChannel()
    {
        var mockChannel = new Mock<INetworkChannel>();

        mockChannel.Setup(c => c.Resume(It.IsAny<Operation>()));

        var channel = mockChannel.Object;
        channel.Resume(Operation.Read);

        mockChannel.Verify(c => c.Resume(Operation.Read), Times.Once);
    }

    [Fact]
    public void NetworkChannel_TryPause_WhenSuccessful_ReturnsTrue()
    {
        var mockChannel = new Mock<INetworkChannel>();
        mockChannel.Setup(c => c.TryPause(It.IsAny<Operation>())).Returns(true);

        var channel = mockChannel.Object;
        var result = channel.TryPause(Operation.Write);

        Assert.True(result);
        mockChannel.Verify(c => c.TryPause(Operation.Write), Times.Once);
    }

    [Fact]
    public void NetworkChannel_TryResume_WhenSuccessful_ReturnsTrue()
    {
        var mockChannel = new Mock<INetworkChannel>();
        mockChannel.Setup(c => c.TryResume(It.IsAny<Operation>())).Returns(true);

        var channel = mockChannel.Object;
        var result = channel.TryResume(Operation.Write);

        Assert.True(result);
        mockChannel.Verify(c => c.TryResume(Operation.Write), Times.Once);
    }

    [Fact]
    public async Task NetworkChannel_FlushAsync_WhenCalled_FlushesChannel()
    {
        var mockChannel = new Mock<INetworkChannel>();

        mockChannel.Setup(c => c.FlushAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var channel = mockChannel.Object;
        await channel.FlushAsync();

        mockChannel.Verify(c => c.FlushAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NetworkStream_ReadAsync_WhenDataAvailable_ReturnsReadBytes()
    {
        var mockNetworkStream = new Mock<INetworkStream>();
        var testData = new byte[] { 1, 2, 3, 4, 5 };
        var buffer = new byte[5];

        mockNetworkStream.Setup(s => s.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Memory<byte> memory, CancellationToken _) =>
            {
                testData.CopyTo(memory);
                return testData.Length;
            });

        var stream = mockNetworkStream.Object;
        var bytesRead = await stream.ReadAsync(buffer);

        Assert.Equal(5, bytesRead);
        Assert.Equal(testData, buffer);
    }

    [Fact]
    public async Task NetworkStream_WriteAsync_WhenCalled_WritesData()
    {
        var mockNetworkStream = new Mock<INetworkStream>();
        var testData = new byte[] { 1, 2, 3, 4, 5 };
        var capturedData = Array.Empty<byte>();

        mockNetworkStream.Setup(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
            .Callback<ReadOnlyMemory<byte>, CancellationToken>((memory, _) => capturedData = memory.ToArray())
            .Returns(ValueTask.CompletedTask);

        var stream = mockNetworkStream.Object;
        await stream.WriteAsync(testData);

        Assert.Equal(testData, capturedData);
        mockNetworkStream.Verify(s => s.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void NetworkStream_PrependBuffer_WhenCalled_BuffersData()
    {
        var mockNetworkStream = new Mock<INetworkStream>();
        var testData = new byte[] { 1, 2, 3 };

        mockNetworkStream.Setup(s => s.PrependBuffer(It.IsAny<Memory<byte>>()));

        var stream = mockNetworkStream.Object;
        stream.PrependBuffer(testData);

        mockNetworkStream.Verify(s => s.PrependBuffer(It.IsAny<Memory<byte>>()), Times.Once);
    }
}
