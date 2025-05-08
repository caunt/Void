using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Serilog;
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
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Api.Servers;
using Void.Proxy.Commands;
using Void.Proxy.Configurations;
using Void.Proxy.Console;
using Void.Proxy.Crypto;
using Void.Proxy.Events;
using Void.Proxy.Extensions;
using Void.Proxy.Links;
using Void.Proxy.Players;
using Void.Proxy.Plugins;
using Void.Proxy.Plugins.Dependencies;
using Void.Proxy.Plugins.Dependencies.Embedded;
using Void.Proxy.Plugins.Dependencies.Extensions;
using Void.Proxy.Plugins.Dependencies.File;
using Void.Proxy.Plugins.Dependencies.Nuget;
using Void.Proxy.Servers;

Console.Title = nameof(Void);
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var configuration = new LoggerConfiguration();
configuration.Enrich.FromLogContext();
configuration.MinimumLevel.ControlledBy(Platform.LoggingLevelSwitch);
configuration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}");

Log.Logger = configuration.CreateLogger();


try
{
    await BuildCommandLine()
        .UseHost(builder => builder
            .UseServiceProviderFactory(new DryIocServiceProviderFactory(new Container(Rules.MicrosoftDependencyInjectionRules)))
            .UseConsoleLifetime(options => options.SuppressStatusMessages = true)
            .ConfigureServices(services => services
                .AddSerilog()
                .AddJsonOptions()
                .AddHttpClient()
                .AddSettings()
                .AddSingletonAndListen<ICryptoService, RsaCryptoService>()
                .AddSingletonAndListen<IEventService, EventService>()
                .AddSingletonAndListen<IPluginService, PluginService>()
                .AddSingletonAndListen<IPlayerService, PlayerService>()
                .AddSingletonAndListen<IServerService, ServerService>()
                .AddSingletonAndListen<ILinkService, LinkService>()
                .AddSingletonAndListen<IConsoleService, ConsoleService>()
                .AddSingletonAndListen<ICommandService, CommandService>()
                .AddSingletonAndListen<IConfigurationService, ConfigurationService>()
                .AddSingletonAndListen<IDependencyService, DependencyService>()
                .AddSingletonAndListen<IProxy, Platform>()
                .AddSingleton<IFileDependencyResolver, FileDependencyResolver>(FileDependencyResolver.Factory)
                .AddSingleton<INuGetDependencyResolver, NuGetDependencyResolver>()
                .AddSingleton<IEmbeddedDependencyResolver, EmbeddedDependencyResolver>()
                .AddHostedService(services => services.GetRequiredService<IConfigurationService>())
                .AddHostedService(services => services.GetRequiredService<IProxy>())))
        .Build()
        .InvokeAsync(args);
}
finally
{
    Log.CloseAndFlush();
}

static CommandLineBuilder BuildCommandLine()
{
    var root = new RootCommand("Runs the proxy");

    PluginService.RegisterOptions(root);

    root.SetHandler(ReadCommands);

    return new CommandLineBuilder(root);
}

static async Task ReadCommands(InvocationContext context)
{
    var host = context.GetHost();
    var console = host.Services.GetRequiredService<IConsoleService>();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
    var token = lifetime.ApplicationStopping;

    console.Setup();

    try
    {
        while (!token.IsCancellationRequested)
            await console.HandleCommandsAsync(token);
    }
    catch (Exception exception) when (exception is TaskCanceledException or OperationCanceledException)
    {
        // Ignore
    }
}
