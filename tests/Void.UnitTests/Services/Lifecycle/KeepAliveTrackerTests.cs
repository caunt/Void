using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_12_1_to_v1_12_2;
using Void.Proxy.Plugins.Common.Network.Registries.Transformations.Mappings;
using Void.Proxy.Plugins.Common.Services.Lifecycle;
using Xunit;

namespace Void.UnitTests.Services.Lifecycle;

public class KeepAliveTrackerTests
{
    [Fact]
    public async Task Pong_WithMatchingId_AllowsNextRequestAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var intervalController = new IntervalController();
        var sentRequestIds = Channel.CreateUnbounded<long>();
        var requestIds = new Queue<long>([41, 42]);
        await using var tracker = CreateTracker(intervalController, (requestId, cancellationToken) => sentRequestIds.Writer.WriteAsync(requestId, cancellationToken).AsTask(), (_, _) => ValueTask.CompletedTask, () => requestIds.Dequeue());

        await intervalController.AdvanceAsync();
        Assert.Equal(41, await sentRequestIds.Reader.ReadAsync(cancellationToken));
        Assert.True(tracker.Pong(41, cancellationToken));

        await intervalController.AdvanceAsync();
        Assert.Equal(42, await sentRequestIds.Reader.ReadAsync(cancellationToken));
    }

    [Fact]
    public async Task Pong_WithMismatchedId_DoesNotPreventTimeoutAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var intervalController = new IntervalController();
        var sentRequestIds = Channel.CreateUnbounded<long>();
        var logger = new RecordingLogger();
        var timeoutCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var tracker = CreateTracker(intervalController, (requestId, cancellationToken) => sentRequestIds.Writer.WriteAsync(requestId, cancellationToken).AsTask(), (_, _) =>
        {
            timeoutCompletionSource.TrySetResult();
            return ValueTask.CompletedTask;
        }, () => 73, logger);

        await intervalController.AdvanceAsync();
        Assert.Equal(73, await sentRequestIds.Reader.ReadAsync(cancellationToken));
        Assert.False(tracker.Pong(74, cancellationToken));
        Assert.Contains(LogLevel.Warning, logger.LogLevels);
        Assert.DoesNotContain(LogLevel.Debug, logger.LogLevels);

        await intervalController.AdvanceAsync();
        await intervalController.AdvanceAsync();
        await intervalController.AdvanceAsync();
        await timeoutCompletionSource.Task;
    }

    [Fact]
    public async Task Pong_WithoutOutstandingRequest_IsRejectedAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var logger = new RecordingLogger();
        await using var tracker = CreateTracker(new IntervalController(), (_, _) => Task.CompletedTask, (_, _) => ValueTask.CompletedTask, () => 73, logger);

        Assert.False(tracker.Pong(73, cancellationToken));
        Assert.Contains(LogLevel.Warning, logger.LogLevels);
        Assert.DoesNotContain(LogLevel.Debug, logger.LogLevels);
    }

    [Fact]
    public async Task DisposeAsync_CancelsWorkerWithoutSendingOrTimingOutAsync()
    {
        var intervalController = new IntervalController();
        var sendCount = 0;
        var timeoutCount = 0;
        var tracker = CreateTracker(intervalController, (_, _) =>
        {
            Interlocked.Increment(ref sendCount);
            return Task.CompletedTask;
        }, (_, _) =>
        {
            Interlocked.Increment(ref timeoutCount);
            return ValueTask.CompletedTask;
        }, () => 1);

        await intervalController.WaitUntilWorkerIsWaitingAsync();
        await tracker.DisposeAsync();

        Assert.Equal(0, sendCount);
        Assert.Equal(0, timeoutCount);
    }

    [Fact]
    public async Task Tracking_PausesWhilePlayerHasNoActiveLinkAsync()
    {
        var intervalController = new IntervalController();
        var sentRequestIds = Channel.CreateUnbounded<long>();
        var timeoutCount = 0;
        var hasActiveLink = false;
        await using var tracker = CreateTracker(intervalController, (requestId, cancellationToken) => sentRequestIds.Writer.WriteAsync(requestId, cancellationToken).AsTask(), (_, _) =>
        {
            Interlocked.Increment(ref timeoutCount);
            return ValueTask.CompletedTask;
        }, () => 81, canTrackKeepAlive: () => hasActiveLink);

        await intervalController.AdvanceAsync();
        Assert.False(sentRequestIds.Reader.TryRead(out _));

        hasActiveLink = true;
        await intervalController.AdvanceAsync();
        Assert.Equal(81, await sentRequestIds.Reader.ReadAsync(TestContext.Current.CancellationToken));

        hasActiveLink = false;
        await intervalController.AdvanceAsync();
        await intervalController.AdvanceAsync();
        await intervalController.AdvanceAsync();
        Assert.Equal(0, timeoutCount);
    }

    [Fact]
    public async Task Registry_RedirectedPlayerKeepsOutstandingRequestAsync()
    {
        var registry = new KeepAliveTrackerRegistry<object>();
        var player = new object();
        var intervalController = new IntervalController();
        var sentRequestIds = Channel.CreateUnbounded<long>();
        var requestIds = new Queue<long>([91, 92]);
        var timeoutCount = 0;
        var creationCount = 0;
        await using var tracker = registry.GetOrAdd(player, () =>
        {
            creationCount++;
            return CreateTracker(intervalController, (requestId, cancellationToken) => sentRequestIds.Writer.WriteAsync(requestId, cancellationToken).AsTask(), (_, _) =>
            {
                Interlocked.Increment(ref timeoutCount);
                return ValueTask.CompletedTask;
            }, () => requestIds.Dequeue());
        });

        await intervalController.AdvanceAsync();
        Assert.Equal(91, await sentRequestIds.Reader.ReadAsync(TestContext.Current.CancellationToken));

        var redirectedTracker = registry.GetOrAdd(player, () =>
        {
            creationCount++;
            return CreateTracker(new IntervalController(), (_, _) => Task.CompletedTask, (_, _) => ValueTask.CompletedTask, () => 93);
        });

        Assert.Equal(1, creationCount);
        Assert.Same(tracker, redirectedTracker);
        Assert.True(redirectedTracker.Pong(91, TestContext.Current.CancellationToken));

        await intervalController.AdvanceAsync();
        Assert.Equal(92, await sentRequestIds.Reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.Equal(0, timeoutCount);
    }

    [Fact]
    public void UsesLegacyRequestIdWidth_ReturnsExpectedBoundary()
    {
        Assert.True(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.MINECRAFT_1_7_2));
        Assert.True(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.MINECRAFT_1_12_1));
        Assert.False(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.MINECRAFT_1_12_2));
        Assert.False(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.Latest));
    }

    [Fact]
    public void IsLegacyTerrainKeepAlive_ReturnsExpectedBoundary()
    {
        Assert.True(KeepAliveTracker.IsLegacyTerrainKeepAlive(ProtocolVersion.MINECRAFT_1_7_2, 0));
        Assert.True(KeepAliveTracker.IsLegacyTerrainKeepAlive(ProtocolVersion.MINECRAFT_1_12_1, 0));
        Assert.False(KeepAliveTracker.IsLegacyTerrainKeepAlive(ProtocolVersion.MINECRAFT_1_12_2, 0));
        Assert.False(KeepAliveTracker.IsLegacyTerrainKeepAlive(ProtocolVersion.MINECRAFT_1_12_1, 1));
    }

    [Fact]
    public void CreateRequestId_ForLegacyVersion_ReturnsSignedIntRange()
    {
        for (var i = 0; i < 256; i++)
        {
            var id = KeepAliveTracker.CreateRequestId(ProtocolVersion.MINECRAFT_1_12_1);

            Assert.InRange(id, int.MinValue, int.MaxValue);
            Assert.NotEqual(0, id);
        }
    }

    [Fact]
    public void LegacyWidthRequestIds_SurviveKeepAliveTransformationRoundTrip()
    {
        var ids = new long[] { int.MinValue, -1, 0, 1962467597, int.MaxValue };

        foreach (var id in ids)
            Assert.Equal(id, TransformLongThroughLegacyWireFormat(id));
    }

    [Fact]
    public void ArbitraryLongRequestId_DoesNotSurviveKeepAliveTransformationRoundTrip()
    {
        const long sentId = 1983115045386117389;
        const long receivedId = 1962467597;

        var transformedId = TransformLongThroughLegacyWireFormat(sentId);

        Assert.Equal(receivedId, transformedId);
        Assert.NotEqual(sentId, transformedId);
    }

    private static long TransformLongThroughLegacyWireFormat(long id)
    {
        var transformation = new KeepAliveTransformation1_12_2();

        using var longStream = new MemoryStream();
        var longBuffer = new MinecraftBuffer(longStream);
        longBuffer.WriteLong(id);
        longStream.Position = 0;

        using var varIntStream = Transform(longStream, transformation.Downgrade);
        using var transformedStream = Transform(varIntStream, transformation.Upgrade);
        var transformedBuffer = new MinecraftBuffer(transformedStream);

        return transformedBuffer.ReadLong();
    }

    private static MemoryStream Transform(MemoryStream inputStream, MinecraftPacketTransformation transformation)
    {
        var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(0, inputStream));
        transformation(wrapper);
        wrapper.Reset();

        var outputStream = new MemoryStream();
        var outputBuffer = new MinecraftBuffer(outputStream);
        wrapper.WriteProcessedValues(ref outputBuffer);
        outputStream.Position = 0;

        return outputStream;
    }

    private static KeepAliveTracker CreateTracker(IntervalController intervalController, KeepAliveTracker.SendKeepAliveRequest sendRequest, KeepAliveTracker.HandleKeepAliveTimeout handleTimeout, KeepAliveTracker.CreateKeepAliveRequestId createRequestId, ILogger? logger = null, Func<bool>? canTrackKeepAlive = null)
    {
        return new KeepAliveTracker(logger ?? NullLogger.Instance, sendRequest, handleTimeout, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), createRequestId, intervalController.WaitAsync, canTrackKeepAlive);
    }

    private sealed class RecordingLogger : ILogger
    {
        public List<LogLevel> LogLevels { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            LogLevels.Add(logLevel);
        }
    }

    private sealed class IntervalController
    {
        private readonly Channel<TaskCompletionSource> _waiters = Channel.CreateUnbounded<TaskCompletionSource>();
        private readonly TaskCompletionSource _workerWaitingCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task AdvanceAsync()
        {
            var waiter = await _waiters.Reader.ReadAsync();
            waiter.TrySetResult();
        }

        public async Task WaitUntilWorkerIsWaitingAsync()
        {
            await _workerWaitingCompletionSource.Task;
        }

        public async Task WaitAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            await _waiters.Writer.WriteAsync(completionSource, cancellationToken);
            _workerWaitingCompletionSource.TrySetResult();
            await completionSource.Task.WaitAsync(cancellationToken);
        }
    }
}
