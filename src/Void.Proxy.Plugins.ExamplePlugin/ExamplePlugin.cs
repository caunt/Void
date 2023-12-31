﻿using Serilog;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ExamplePlugin;

public class ExamplePlugin : IPlugin
{
    public required ILogger Logger { get; init; }
    public string Name => nameof(ExamplePlugin);

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [Subscribe]
    public void OnProxyStart(ProxyStart @event)
    {
        Logger.Information("Received ProxyStart event");
    }

    [Subscribe]
    public void OnProxyStop(ProxyStop @event)
    {
        Logger.Information("Received ProxyStop event");
    }
}
