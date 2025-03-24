﻿using Serilog;
using Serilog.Events;
using System.Diagnostics;
using Void.Proxy;
using Void.Proxy.Api;
using Void.Proxy.Api.Crypto;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Forwarding;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Plugins.Services;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;
using Void.Proxy.Crypto;
using Void.Proxy.Events;
using Void.Proxy.Forwarding;
using Void.Proxy.Links;
using Void.Proxy.Players;
using Void.Proxy.Plugins;
using Void.Proxy.Servers;
using Void.Proxy.Settings;

const int width = 165;

if (Debugger.IsAttached && OperatingSystem.IsWindows() && Console.WindowWidth < width)
    Console.WindowWidth = width;

var configuration = new LoggerConfiguration();
configuration.Enrich.FromLogContext();
configuration.MinimumLevel.ControlledBy(Platform.LoggingLevelSwitch);
configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
configuration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}");

Log.Logger = configuration.CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog();
    builder.Services.AddJsonOptions();
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<ISettings, Settings>();
    builder.Services.AddSingleton<ICryptoService, RsaCryptoService>();
    builder.Services.AddSingleton<IEventService, EventService>();
    builder.Services.AddSingleton<IPluginDependencyService, PluginDependencyService>();
    builder.Services.AddSingleton<IPluginService, PluginService>();
    builder.Services.AddSingleton<IPlayerService, PlayerService>();
    builder.Services.AddSingleton<IServerService, ServerService>();
    builder.Services.AddSingleton<ILinkService, LinkService>();
    builder.Services.AddSingleton<IForwardingService, ForwardingService>();
    builder.Services.AddSingleton<IProxy, Platform>();
    builder.Services.AddHostedService<Platform>();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}