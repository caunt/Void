using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public class KeepAliveTracker : IDisposable
{
    private readonly Lock _stateLock = new();
    private CancellationToken _timeoutCancellationToken;
    private IPlayer? _timeoutPlayer;
    private bool _requestOutstanding;
    private long _requestId = DefaultRequestId;

    public const int DefaultRequestId = -1;
    public delegate Task SendKeepAliveRequest(long id);
    public delegate long CreateKeepAliveRequestId();

    public RateLimitedFunc<IPlayer, CancellationToken, Task<bool>> DebouncerCallback { get; }
    public System.Timers.Timer Sender { get; }

    public KeepAliveTracker(SendKeepAliveRequest sendRequestFunction, TimeSpan keepAliveRequestInterval, TimeSpan keepAliveResponseTimeout = default, CreateKeepAliveRequestId? createRequestIdFunction = null)
        : this(sendRequestFunction, keepAliveRequestInterval, keepAliveResponseTimeout, createRequestIdFunction, () => true)
    {
    }

    internal KeepAliveTracker(SendKeepAliveRequest sendRequestFunction, TimeSpan keepAliveRequestInterval, TimeSpan keepAliveResponseTimeout, CreateKeepAliveRequestId? createRequestIdFunction, Func<bool> canSendKeepAliveFunction)
    {
        if (keepAliveResponseTimeout == default)
            keepAliveResponseTimeout = keepAliveRequestInterval * 3;

        createRequestIdFunction ??= CreateRequestId;
        var debouncerCallback = Debouncer.Debounce<IPlayer, CancellationToken, Task<bool>>(HandleTimeoutAsync, keepAliveResponseTimeout);

        var timer = new System.Timers.Timer(keepAliveRequestInterval) { AutoReset = false };
        timer.Elapsed += (sender, eventArgs) =>
        {
            CancellationToken timeoutCancellationToken = default;
            IPlayer? timeoutPlayer = null;
            long? requestId = null;

            using (_stateLock.EnterScope())
            {
                if (_requestOutstanding)
                    return;

                if (canSendKeepAliveFunction())
                {
                    _requestOutstanding = true;
                    requestId = _requestId = createRequestIdFunction();

                    if (_timeoutPlayer is { } currentPlayer)
                        timeoutPlayer = currentPlayer;

                    timeoutCancellationToken = _timeoutCancellationToken;
                }
            }

            if (requestId is { } value)
            {
                _ = sendRequestFunction(value);

                if (timeoutPlayer is { } timeoutTarget)
                    debouncerCallback.Invoke(timeoutTarget, timeoutCancellationToken);
            }
            else
                timer.Start();
        };
        timer.Start();

        DebouncerCallback = debouncerCallback;
        Sender = timer;
    }

    public static long CreateRequestId(ProtocolVersion protocolVersion)
    {
        if (UsesLegacyRequestIdWidth(protocolVersion))
        {
            var requestId = Random.Shared.NextInt64(int.MinValue, int.MaxValue);
            return requestId >= 0 ? requestId + 1 : requestId;
        }

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

    public async ValueTask PongAsync(IPlayer player, long id, CancellationToken cancellationToken)
    {
        player.Logger.LogTrace("Keep Alive hit {Id} received", id);

        long expectedRequestId;
        bool requestOutstanding;

        using (_stateLock.EnterScope())
        {
            expectedRequestId = _requestId;
            requestOutstanding = _requestOutstanding;

            if (requestOutstanding && expectedRequestId == id)
                _requestOutstanding = false;
        }

        if (!requestOutstanding)
        {
            player.Logger.LogWarning("Keep Alive hit {Id} when no request is outstanding", id);
            return;
        }

        if (expectedRequestId != id)
        {
            player.Logger.LogWarning("Keep Alive hit {Id} does not match outstanding id {ExpectedId}", id, expectedRequestId);
            return;
        }

        await RefreshAsync(player, cancellationToken);
    }

    internal async ValueTask PongAsync(IPlayer player, CancellationToken cancellationToken)
    {
        player.Logger.LogTrace("Keep Alive response with legacy zero id received");

        using (_stateLock.EnterScope())
            _requestOutstanding = false;

        await RefreshAsync(player, cancellationToken);
    }

    internal ValueTask RefreshAsync(IPlayer player, CancellationToken cancellationToken)
    {
        player.Logger.LogTrace("Refreshing Keep Alive");

        using (_stateLock.EnterScope())
        {
            _timeoutPlayer = player;
            _timeoutCancellationToken = cancellationToken;

            if (!_requestOutstanding)
            {
                Sender.Stop();
                Sender.Start();
            }
        }

        return ValueTask.CompletedTask;
    }

    private async Task<bool> HandleTimeoutAsync(IPlayer player, CancellationToken cancellationToken)
    {
        try
        {
            using (_stateLock.EnterScope())
                if (!_requestOutstanding)
                    return false;

            if (player.Link is not { IsAlive: true })
                return false;

            DebouncerCallback.Dispose();
            player.Logger.LogInformation("Keep alive timed out");
            await player.KickAsync("Timed out", cancellationToken);
        }
        catch (Exception exception)
        {
            player.Logger.LogError(exception, "Error while handling keep alive timeout");
        }

        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Sender.Stop();

        Sender.Dispose();
        DebouncerCallback.Dispose();
    }
}
