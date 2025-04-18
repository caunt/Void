﻿using Serilog;
using Serilog.Events;
using Void.Minecraft.Players.Extensions;
using Void.Proxy;
using Void.Proxy.Api;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Crypto;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Forwarding;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;
using Void.Proxy.Commands;
using Void.Proxy.Configurations;
using Void.Proxy.Console;
using Void.Proxy.Crypto;
using Void.Proxy.Events;
using Void.Proxy.Forwarding;
using Void.Proxy.Links;
using Void.Proxy.Players;
using Void.Proxy.Plugins;
using Void.Proxy.Plugins.Dependencies;
using Void.Proxy.Servers;
using Void.Proxy.Settings;

Console.Title = nameof(Void);
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var configuration = new LoggerConfiguration();
configuration.Enrich.FromLogContext();
configuration.MinimumLevel.ControlledBy(Platform.LoggingLevelSwitch);
configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
configuration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}");

Log.Logger = configuration.CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.Configure<HostOptions>(options =>
    {
        options.ServicesStartConcurrently = true;
        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
    });

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
    builder.Services.AddSingleton<IConsoleService, ConsoleService>();
    builder.Services.AddSingleton<ICommandService, CommandService>();
    builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
    builder.Services.AddSingleton<IDependencyService, DependencyService>();
    builder.Services.AddSingleton<IProxy, Platform>();
    builder.Services.AddHostedService(services => services.GetRequiredService<IConfigurationService>());
    builder.Services.AddHostedService(services => services.GetRequiredService<IProxy>());

    builder.Services.AddScoped(provider => provider.GetRequiredService<IPlayer>().AsMinecraftPlayer());
    builder.Services.AddScoped(provider =>
    {
        var players = provider.GetRequiredService<IPlayerService>();

        var player = players.All.FirstOrDefault(player => player.Context.Services == provider) ??
            throw new InvalidOperationException("Player not found.");

        return player;
    });

    builder.Services.RegisterListeners();

    var host = builder.Build();

    var console = host.Services.GetRequiredService<IConsoleService>();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
    var token = lifetime.ApplicationStopping;

    console.Setup();
    var app = host.RunAsync();

    try
    {
        while (!token.IsCancellationRequested)
            await console.HandleCommandsAsync(token);
    }
    catch (Exception exception) when (exception is TaskCanceledException or OperationCanceledException)
    {
        // Ignore
    }

    await app;
}
finally
{
    Log.CloseAndFlush();
}
