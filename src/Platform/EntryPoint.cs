using Serilog;
using Serilog.Events;
using Void.Proxy;
using Void.Proxy.API;
using Void.Proxy.API.Crypto;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Forwarding;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network.Protocol.Services;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Registries.Packets;
using Void.Proxy.API.Servers;
using Void.Proxy.API.Settings;
using Void.Proxy.Crypto;
using Void.Proxy.Events;
using Void.Proxy.Forwarding;
using Void.Proxy.Links;
using Void.Proxy.Network.Protocol;
using Void.Proxy.Players;
using Void.Proxy.Plugins;
using Void.Proxy.Registries.Packets;
using Void.Proxy.Servers;
using Void.Proxy.Settings;

if (OperatingSystem.IsWindows())
    Console.WindowWidth = 165;

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.ControlledBy(Platform.LoggingLevelSwitch).MinimumLevel.Override("Microsoft", LogEventLevel.Warning).WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}").CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog();
    builder.Services.AddJsonOptions();
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<ISettings, Settings>();
    builder.Services.AddSingleton<ICryptoService, RsaCryptoService>();
    builder.Services.AddSingleton<IEventService, EventService>();
    builder.Services.AddSingleton<IPluginService, PluginService>();
    builder.Services.AddSingleton<IPlayerService, PlayerService>();
    builder.Services.AddSingleton<IServerService, ServerService>();
    builder.Services.AddSingleton<ILinkService, LinkService>();
    builder.Services.AddSingleton<IForwardingService, ForwardingService>();
    builder.Services.AddSingleton<IProxy, Platform>();
    builder.Services.AddHostedService<Platform>();
    builder.Services.AddScoped<IChannelBuilderService, ChannelBuilderService>();
    builder.Services.AddScoped<IPacketRegistryHolder, PacketRegistryHolder>();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}