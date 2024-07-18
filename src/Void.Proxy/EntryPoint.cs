using Serilog;
using Void.Proxy;
using Void.Proxy.API;
using Void.Proxy.API.Crypto;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network.Protocol.Services;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Servers;
using Void.Proxy.API.Settings;
using Void.Proxy.Crypto;
using Void.Proxy.Events;
using Void.Proxy.Links;
using Void.Proxy.Network.Protocol.Services;
using Void.Proxy.Players;
using Void.Proxy.Plugins;
using Void.Proxy.Servers;
using Void.Proxy.Settings;

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
    .MinimumLevel.ControlledBy(Platform.LoggingLevelSwitch)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}")
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog()
    .AddJsonOptions()
    .AddHttpClient()
    .AddSingleton<ISettings, Settings>()
    .AddSingleton<ICryptoService, RsaCryptoService>()
    .AddSingleton<IEventService, EventService>()
    .AddSingleton<IPluginService, PluginService>()
    .AddSingleton<IPlayerService, PlayerService>()
    .AddSingleton<IServerService, ServerService>()
    .AddSingleton<ILinkService, LinkService>()
    .AddSingleton<IProxy, Platform>()
    .AddHostedService<Platform>()
    .AddScoped<IChannelBuilderService, ChannelBuilderService>();

var host = builder.Build();
await host.RunAsync();