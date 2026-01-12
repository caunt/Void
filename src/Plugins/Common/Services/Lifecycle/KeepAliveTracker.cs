using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public class KeepAliveTracker : IDisposable
{
    private long _requestId = DefaultRequestId;

    public const int DefaultRequestId = -1;
    public delegate Task SendKeepAliveRequest(long id);

    public RateLimitedFunc<IPlayer, CancellationToken, Task<bool>> DebouncerCallback { get; }
    public System.Timers.Timer Sender { get; }

    public KeepAliveTracker(SendKeepAliveRequest sendRequestFunction, TimeSpan keepAliveRequestInterval, TimeSpan keepAliveResponseTimeout = default)
    {
        if (keepAliveResponseTimeout == default)
            keepAliveResponseTimeout = keepAliveRequestInterval * 3;

        var timer = new System.Timers.Timer(keepAliveRequestInterval);
        timer.Elapsed += (eventArgs, sender) => _ = sendRequestFunction(_requestId = Random.Shared.NextInt64()); // TODO: Handle exceptions?
        timer.Start();

        DebouncerCallback = Debouncer.Debounce<IPlayer, CancellationToken, Task<bool>>(HandleTimeoutAsync, keepAliveResponseTimeout);
        Sender = timer;
    }

    public async ValueTask PongAsync(IPlayer player, long id, CancellationToken cancellationToken)
    {
        player.Logger.LogTrace("Keep Alive hit {Id} received", id);

        if (_requestId != id)
            player.Logger.LogWarning("Keep Alive hit {Id} does not match last sent id {LastId}", id, _requestId);

        if (DebouncerCallback.Invoke(player, cancellationToken) is not { } timedoutTask)
            return;

        if (!await timedoutTask)
            return;

        throw new Exception("Keep Alive pong invoked when player is already timed out.");
    }

    private async Task<bool> HandleTimeoutAsync(IPlayer player, CancellationToken cancellationToken)
    {
        try
        {
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
