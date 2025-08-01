﻿using System.Diagnostics.CodeAnalysis;
using Nito.AsyncEx;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Network.Streams;
using Void.Proxy.Api.Network.Streams.Manual.Binary;
using Void.Proxy.Api.Network.Streams.Manual.Network;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;

namespace Void.Proxy.Plugins.Common.Network.Channels;

public class SimpleMinecraftChannel(IMessageStreamBase head) : INetworkChannel
{
    private readonly AsyncLock _writeLock = new();
    private TaskCompletionSource? _readPause;
    private TaskCompletionSource? _writePause;

    public bool CanRead => head.CanRead;
    public bool CanWrite => head.CanWrite;

    public IMessageStreamBase Head => head;

    public bool IsAlive => head.IsAlive;
    public bool IsConfigured => head is IMessageStream;
    public bool IsPaused => this is { _readPause.Task.IsCompleted: false } or { _writePause.Task.IsCompleted: false };

    public void Add<T>() where T : class, IMessageStream, new()
    {
        Add(new T());
    }

    public void Add<T>(T stream) where T : class, IMessageStream
    {
        stream.BaseStream = head;
        head = stream;
    }

    public void AddBefore<TBefore, TValue>() where TBefore : class, IMessageStream where TValue : class, IMessageStream, new()
    {
        AddBefore<TBefore, TValue>(new TValue());
    }

    public void AddBefore<TBefore, TValue>(TValue stream) where TBefore : class, IMessageStream where TValue : class, IMessageStream
    {
        var before = Get<TBefore>();
        var beforeBaseStream = before.BaseStream;

        stream.BaseStream = beforeBaseStream;
        before.BaseStream = stream;
    }

    public void Remove<T>() where T : class, IMessageStream, new()
    {
        Remove(Get<T>());
    }

    public void Remove<T>(T value) where T : class, IMessageStream
    {
        if (head != value)
        {
            var previousStreamBase = head;
            var currentStreamBase = previousStreamBase;

            while (currentStreamBase is not null)
            {
                if (currentStreamBase is not IMessageStream currentStream || previousStreamBase is not IMessageStream previousStream)
                    break;

                if (currentStream == value)
                    previousStream.BaseStream = currentStream.BaseStream;

                previousStreamBase = currentStream;
                currentStreamBase = currentStream.BaseStream;
            }
        }
        else if (value.BaseStream is not null)
        {
            head = value.BaseStream;
        }
        else
        {
            throw new InvalidOperationException($"Cannot remove {value} stream with unset BaseStream");
        }
    }

    public T Get<T>() where T : class, IMessageStreamBase
    {
        if (TryGet<T>(out var stream))
            return stream;

        throw new InvalidOperationException($"{typeof(T)} not found in channel");
    }

    public bool Has<T>() where T : class, IMessageStreamBase
    {
        return TryGet<T>(out _);
    }

    public bool TryGet<T>([MaybeNullWhen(false)] out T result) where T : class, IMessageStreamBase
    {
        return Get(head, out result);
    }

    public void PrependBuffer(Memory<byte> memory)
    {
        var stream = Get<INetworkStream>();
        stream.PrependBuffer(memory);
    }

    public async ValueTask<INetworkMessage> ReadMessageAsync(Side origin, CancellationToken cancellationToken = default)
    {
        if (_readPause is not null)
            await _readPause.Task.WaitAsync(cancellationToken);

        return head switch
        {
            IMinecraftPacketMessageStream stream => await stream.ReadPacketAsync(origin, cancellationToken),
            ICompleteMessageStream stream => await stream.ReadMessageAsync(cancellationToken),
            IBufferedMessageStream stream => await stream.ReadAsMessageAsync(cancellationToken: cancellationToken),
            _ => throw new InvalidOperationException($"{head.GetType()} cannot be used to read messages")
        };
    }

    public async ValueTask WriteMessageAsync(INetworkMessage message, Side origin, CancellationToken cancellationToken = default)
    {
        if (_writePause is not null)
            await _writePause.Task.WaitAsync(cancellationToken);

        // Underlying streams often non thread-safe at writes
        using var _ = await _writeLock.LockAsync(cancellationToken);

        switch (head)
        {
            case IMinecraftPacketMessageStream stream:
                await stream.WritePacketAsync(message as IMinecraftPacket ?? throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(IMinecraftPacket)}."), origin, cancellationToken);
                break;
            case ICompleteMessageStream stream:
                await stream.WriteMessageAsync(message as CompleteBinaryMessage ?? throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(CompleteBinaryMessage)}."), cancellationToken);
                break;
            case IBufferedMessageStream stream:
                await stream.WriteAsMessageAsync(message as BufferedBinaryMessage ?? throw new InvalidCastException($"Unable to cast object of type {message.GetType()} to type {typeof(BufferedBinaryMessage)}."), cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"{head.GetType()} cannot be used to write messages");
        }
    }

    public void Pause(Operation operation = Operation.Read)
    {
        if (!TryPause(operation))
            throw new InvalidOperationException($"{nameof(INetworkChannel)} is already paused");
    }

    public bool TryPause(Operation operation = Operation.Read)
    {
        switch (operation)
        {
            case Operation.Read:
                if (_readPause is { Task.IsCompleted: false })
                    return false;

                _readPause = new TaskCompletionSource();
                break;
            case Operation.Write:
                if (_writePause is { Task.IsCompleted: false })
                    return false;

                _writePause = new TaskCompletionSource();
                break;
            case Operation.Any:
                if (_readPause is { Task.IsCompleted: false } || _writePause is { Task.IsCompleted: false })
                    return false;

                _readPause = new TaskCompletionSource();
                _writePause = new TaskCompletionSource();
                break;
        }

        return true;
    }

    public void Resume(Operation operation = Operation.Read)
    {
        if (!TryResume(operation))
            throw new InvalidOperationException($"{nameof(INetworkChannel)} is not paused");
    }

    public bool TryResume(Operation operation = Operation.Read)
    {
        switch (operation)
        {
            case Operation.Read:
                if (_readPause is null or { Task.IsCompleted: true })
                    return false;

                _readPause.SetResult();
                break;
            case Operation.Write:
                if (_writePause is null or { Task.IsCompleted: true })
                    return false;

                _writePause.SetResult();
                break;
            case Operation.Any:
                if (_readPause is null or { Task.IsCompleted: true } || _writePause is null or { Task.IsCompleted: true })
                    return false;

                _readPause.SetResult();
                _writePause.SetResult();
                break;
        }

        return true;
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await head.FlushAsync(cancellationToken);
    }

    public void Close()
    {
        head.Close();
    }

    public void Dispose()
    {
        head.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await head.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void Flush()
    {
        head.Flush();
    }

    private bool Get<T>(IMessageStreamBase? baseStream, [MaybeNullWhen(false)] out T result) where T : class, IMessageStreamBase
    {
        var current = baseStream ?? head;

        while (true)
            switch (current)
            {
                case T found:
                    result = found;
                    return true;
                case IMessageStream stream:
                    current = stream.BaseStream;
                    break;
                default:
                    result = null;
                    return false;
            }
    }
}
