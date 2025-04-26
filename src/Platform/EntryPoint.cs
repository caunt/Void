using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Serilog;
using Serilog.Events;
using Void.Proxy;
using Void.Proxy.Api;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Crypto;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;
using Void.Proxy.Commands;
using Void.Proxy.Configurations;
using Void.Proxy.Console;
using Void.Proxy.Crypto;
using Void.Proxy.Events;
using Void.Proxy.Links;
using Void.Proxy.Players;
using Void.Proxy.Players.Contexts;
using Void.Proxy.Plugins;
using Void.Proxy.Plugins.Dependencies;
using Void.Proxy.Plugins.Dependencies.Extensions;
using Void.Proxy.Servers;

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

    builder.ConfigureContainer(new DryIocServiceProviderFactory(new Container(Rules.MicrosoftDependencyInjectionRules)));

    builder.Services.Configure<HostOptions>(options =>
    {
        options.ServicesStartConcurrently = true;
        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
    });

    builder.Services.AddSerilog();
    builder.Services.AddJsonOptions();
    builder.Services.AddHttpClient();
    builder.Services.AddSingletonAndListen<ICryptoService, RsaCryptoService>();
    builder.Services.AddSingletonAndListen<IEventService, EventService>();
    builder.Services.AddSingletonAndListen<IPluginService, PluginService>();
    builder.Services.AddSingletonAndListen<IPlayerService, PlayerService>();
    builder.Services.AddSingletonAndListen<IServerService, ServerService>();
    builder.Services.AddSingletonAndListen<ILinkService, LinkService>();
    builder.Services.AddSingletonAndListen<IConsoleService, ConsoleService>();
    builder.Services.AddSingletonAndListen<ICommandService, CommandService>();
    builder.Services.AddSingletonAndListen<IConfigurationService, ConfigurationService>();
    builder.Services.AddSingletonAndListen<IDependencyService, DependencyService>();
    builder.Services.AddSingletonAndListen<IProxy, Platform>();
    builder.Services.AddHostedService(services => services.GetRequiredService<IConfigurationService>());
    builder.Services.AddHostedService(services => services.GetRequiredService<IProxy>());

    builder.Services.AddScoped<IPlayerContextAccessor, PlayerContextAccessor>();
    builder.Services.AddScoped(services => services.GetRequiredService<IPlayerContextAccessor>().Context?.Player ?? throw new InvalidOperationException("Player context is not set"));

    builder.Services.AddSingleton<ISettings>(services =>
    {
        return services.GetRequiredService<IConfigurationService>().GetAsync<Settings>().AsTask().Result;
    });

    using var host = builder.Build();

    var console = host.Services.GetRequiredService<IConsoleService>();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
    var token = lifetime.ApplicationStopping;

    console.Setup();

    try
    {
        await host.StartAsync();
    }
    catch (ContainerException containerException)
    {
        throw new Exception(containerException.TryGetDetails(host.Services.GetRequiredService<IContainer>()), containerException);
    }

    try
    {
        while (!token.IsCancellationRequested)
            await console.HandleCommandsAsync(token);
    }
    catch (Exception exception) when (exception is TaskCanceledException or OperationCanceledException)
    {
        // Ignore
    }

    await host.StopAsync();
}
finally
{
    Log.CloseAndFlush();
}
