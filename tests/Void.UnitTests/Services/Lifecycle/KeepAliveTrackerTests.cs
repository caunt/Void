using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
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
        var timeoutCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var tracker = CreateTracker(intervalController, (requestId, cancellationToken) => sentRequestIds.Writer.WriteAsync(requestId, cancellationToken).AsTask(), (_, _) =>
        {
            timeoutCompletionSource.TrySetResult();
            return ValueTask.CompletedTask;
        }, () => 73);

        await intervalController.AdvanceAsync();
        Assert.Equal(73, await sentRequestIds.Reader.ReadAsync(cancellationToken));
        Assert.False(tracker.Pong(74, cancellationToken));

        await intervalController.AdvanceAsync();
        await intervalController.AdvanceAsync();
        await intervalController.AdvanceAsync();
        await timeoutCompletionSource.Task;
    }

    [Fact]
    public async Task Pong_WithoutOutstandingRequest_IsRejectedAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var tracker = CreateTracker(new IntervalController(), (_, _) => Task.CompletedTask, (_, _) => ValueTask.CompletedTask, () => 73);

        Assert.False(tracker.Pong(73, cancellationToken));
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
    public async Task Registry_RemovedLinkCannotAcknowledgeOrKickReplacementLinkAsync()
    {
        var registry = new KeepAliveTrackerRegistry<object>();
        var oldLink = new object();
        var replacementLink = new object();
        var oldIntervalController = new IntervalController();
        var timeoutHandled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var staleKickCount = 0;
        var creationCount = 0;
        var oldTracker = registry.GetOrAdd(oldLink, () =>
        {
            creationCount++;
            return CreateTracker(oldIntervalController, (_, _) => Task.CompletedTask, (tracker, _) =>
            {
                if (registry.IsCurrent(oldLink, tracker))
                    Interlocked.Increment(ref staleKickCount);

                timeoutHandled.TrySetResult();
                return ValueTask.CompletedTask;
            }, () => 91);
        });

        await oldIntervalController.AdvanceAsync();
        Assert.Same(oldTracker, registry.Remove(oldLink));
        Assert.Null(registry.Get(oldLink));

        var replacementTracker = registry.GetOrAdd(replacementLink, () =>
        {
            creationCount++;
            return CreateTracker(new IntervalController(), (_, _) => Task.CompletedTask, (_, _) => ValueTask.CompletedTask, () => 92);
        });

        Assert.Equal(2, creationCount);
        Assert.True(registry.IsCurrent(replacementLink, replacementTracker));
        Assert.False(registry.IsCurrent(oldLink, oldTracker));

        await oldIntervalController.AdvanceAsync();
        await oldIntervalController.AdvanceAsync();
        await oldIntervalController.AdvanceAsync();
        await timeoutHandled.Task;

        Assert.Equal(0, staleKickCount);
        await oldTracker.DisposeAsync();
        await replacementTracker.DisposeAsync();
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
    public void CreateRequestId_ForLegacyVersion_ReturnsSignedIntRange()
    {
        for (var i = 0; i < 256; i++)
        {
            var id = KeepAliveTracker.CreateRequestId(ProtocolVersion.MINECRAFT_1_12_1);

            Assert.InRange(id, int.MinValue, int.MaxValue);
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

    private static KeepAliveTracker CreateTracker(IntervalController intervalController, KeepAliveTracker.SendKeepAliveRequest sendRequest, KeepAliveTracker.HandleKeepAliveTimeout handleTimeout, KeepAliveTracker.CreateKeepAliveRequestId createRequestId)
    {
        return new KeepAliveTracker(NullLogger.Instance, sendRequest, handleTimeout, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), createRequestId, intervalController.WaitAsync);
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
