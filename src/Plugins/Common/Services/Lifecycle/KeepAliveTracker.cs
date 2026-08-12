using Microsoft.Extensions.Logging;
using Void.Minecraft.Network;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public class KeepAliveTracker : IDisposable, IAsyncDisposable
{
    private readonly Lock _lock = new();
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly SendKeepAliveRequest _sendRequestFunction;
    private readonly HandleKeepAliveTimeout _handleTimeoutFunction;
    private readonly CreateKeepAliveRequestId _createRequestIdFunction;
    private readonly WaitForKeepAliveInterval _waitForKeepAliveIntervalFunction;
    private readonly TimeSpan _keepAliveRequestInterval;
    private readonly int _responseIntervalCount;
    private readonly Task _workerTask;
    private long _requestId = DefaultRequestId;
    private int _unansweredIntervalCount;
    private bool _hasOutstandingRequest;
    private int _disposed;

    public const int DefaultRequestId = -1;
    public delegate Task SendKeepAliveRequest(long id, CancellationToken cancellationToken);
    public delegate ValueTask HandleKeepAliveTimeout(KeepAliveTracker tracker, CancellationToken cancellationToken);
    public delegate long CreateKeepAliveRequestId();
    internal delegate Task WaitForKeepAliveInterval(TimeSpan interval, CancellationToken cancellationToken);

    public KeepAliveTracker(ILogger logger, SendKeepAliveRequest sendRequestFunction, HandleKeepAliveTimeout handleTimeoutFunction, TimeSpan keepAliveRequestInterval, TimeSpan keepAliveResponseTimeout = default, CreateKeepAliveRequestId? createRequestIdFunction = null)
        : this(logger, sendRequestFunction, handleTimeoutFunction, keepAliveRequestInterval, keepAliveResponseTimeout, createRequestIdFunction, Task.Delay)
    {
    }

    internal KeepAliveTracker(ILogger logger, SendKeepAliveRequest sendRequestFunction, HandleKeepAliveTimeout handleTimeoutFunction, TimeSpan keepAliveRequestInterval, TimeSpan keepAliveResponseTimeout, CreateKeepAliveRequestId? createRequestIdFunction, WaitForKeepAliveInterval waitForKeepAliveIntervalFunction)
    {
        if (keepAliveRequestInterval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(keepAliveRequestInterval));

        if (keepAliveResponseTimeout == default)
            keepAliveResponseTimeout = keepAliveRequestInterval * 3;

        if (keepAliveResponseTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(keepAliveResponseTimeout));

        _logger = logger;
        _sendRequestFunction = sendRequestFunction;
        _handleTimeoutFunction = handleTimeoutFunction;
        _createRequestIdFunction = createRequestIdFunction ?? CreateRequestId;
        _waitForKeepAliveIntervalFunction = waitForKeepAliveIntervalFunction;
        _keepAliveRequestInterval = keepAliveRequestInterval;
        _responseIntervalCount = Math.Max(1, (int)Math.Ceiling(keepAliveResponseTimeout / keepAliveRequestInterval));
        _workerTask = RunAsync(_cancellationTokenSource.Token);
    }

    public static long CreateRequestId(ProtocolVersion protocolVersion)
    {
        if (UsesLegacyRequestIdWidth(protocolVersion))
            return Random.Shared.NextInt64(int.MinValue, (long)int.MaxValue + 1);

        return CreateRequestId();
    }

    public static long CreateRequestId()
    {
        return Random.Shared.NextInt64();
    }

    public static bool UsesLegacyRequestIdWidth(ProtocolVersion protocolVersion)
    {
        return protocolVersion < ProtocolVersion.MINECRAFT_1_12_2;
    }

    public bool Pong(long id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.LogTrace("Keep Alive hit {Id} received", id);

        using var _ = _lock.EnterScope();

        if (!_hasOutstandingRequest || _requestId != id)
        {
            _logger.LogDebug("Keep Alive hit {Id} does not match outstanding id {LastId}", id, _hasOutstandingRequest ? _requestId : DefaultRequestId);
            return false;
        }

        _hasOutstandingRequest = false;
        _unansweredIntervalCount = 0;
        return true;
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                await _waitForKeepAliveIntervalFunction(_keepAliveRequestInterval, cancellationToken);

                long requestId = default;
                var shouldSendRequest = false;
                var shouldHandleTimeout = false;

                using (var _ = _lock.EnterScope())
                {
                    if (_hasOutstandingRequest)
                    {
                        _unansweredIntervalCount++;
                        shouldHandleTimeout = _unansweredIntervalCount >= _responseIntervalCount;
                    }
                    else
                    {
                        requestId = _requestId = _createRequestIdFunction();
                        _hasOutstandingRequest = true;
                        _unansweredIntervalCount = 0;
                        shouldSendRequest = true;
                    }
                }

                if (shouldHandleTimeout)
                {
                    await _handleTimeoutFunction(this, cancellationToken);
                    return;
                }

                if (shouldSendRequest)
                    await SendAsync(requestId, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private async Task SendAsync(long requestId, CancellationToken cancellationToken)
    {
        try
        {
            await _sendRequestFunction(requestId, cancellationToken);
        }
        catch
        {
            using var _ = _lock.EnterScope();

            if (_hasOutstandingRequest && _requestId == requestId)
            {
                _hasOutstandingRequest = false;
                _unansweredIntervalCount = 0;
            }

            throw;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        GC.SuppressFinalize(this);
        _cancellationTokenSource.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();

        try
        {
            await _workerTask;
        }
        finally
        {
            _cancellationTokenSource.Dispose();
        }
    }
}
