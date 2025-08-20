﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Serilog;
using Serilog.Core;
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

namespace Void.Proxy;

public static class EntryPoint
{
    public record RunOptions : IRunOptions
    {
        public static RunOptions Default { get; } = new();

        public string WorkingDirectory { get; init; } = AppContext.BaseDirectory;
        public string[] Arguments { get; init; } = [];
        public TextWriter? LogWriter { get; init; } = null;
    }

    static EntryPoint() => System.Console.Title = nameof(Void);

    private static async Task<int> Main(string[] args)
    {
        var options = new RunOptions { Arguments = args };
        return await RunAsync(options);
    }

    public static async Task<int> RunAsync(CancellationToken cancellationToken = default)
    {
        return await RunAsync(RunOptions.Default, cancellationToken);
    }

    public static async Task<int> RunAsync(RunOptions options, CancellationToken cancellationToken = default)
    {
        if (options.WorkingDirectory.Equals(RunOptions.Default.WorkingDirectory, StringComparison.OrdinalIgnoreCase))
            Directory.SetCurrentDirectory(options.WorkingDirectory);

        var logger = ConfigureLogging(options.LogWriter);

        try
        {
            return await BuildCommandLine(cancellationToken)
                .UseDefaults()
                .UseHost(builder => SetupHost(builder, logger, options))
                .Build()
                .InvokeAsync(options.Arguments);
        }
        finally
        {
            logger.Dispose();
        }

        static Logger ConfigureLogging(TextWriter? logWriter)
        {
            var configuration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(Platform.LoggingLevelSwitch);

            const string template = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}";

            if (logWriter is null)
                configuration = configuration.WriteTo.Console(outputTemplate: template);
            else
                configuration = configuration.WriteTo.TextWriter(logWriter, outputTemplate: template);

            return configuration.CreateLogger();
        }

        static void SetupHost(IHostBuilder builder, Logger logger, RunOptions options)
        {
            builder
                .UseServiceProviderFactory(new DryIocServiceProviderFactory(new Container(Rules.MicrosoftDependencyInjectionRules)))
                .UseConsoleLifetime(options => options.SuppressStatusMessages = true)
                .ConfigureServices(services => services
                    .AddSerilog(logger, dispose: false)
                    .AddJsonOptions()
                    .AddHttpClient()
                    .AddSettings()
                    .AddSingleton(new ConsoleConfiguration(options.LogWriter is null))
                    .AddSingleton<IRunOptions>(options)
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
                    .AddHostedService(services => (Platform)services.GetRequiredService<IProxy>()));
        }
    }

    private static CommandLineBuilder BuildCommandLine(CancellationToken cancellationToken)
    {
        var root = new RootCommand("Runs the proxy");

        NuGetDependencyResolver.RegisterOptions(root);
        PluginService.RegisterOptions(root);
        ServerService.RegisterOptions(root);
        Platform.RegisterOptions(root);

        root.SetHandler(async context => await MainHandlerAsync(context, cancellationToken));

        return new CommandLineBuilder(root);
    }

    private static async Task MainHandlerAsync(InvocationContext context, CancellationToken cancellationToken)
    {
        var host = context.GetHost();

        var console = host.Services.GetRequiredService<IConsoleService>();
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        var token = lifetime.ApplicationStopping;
        using var registration = cancellationToken.Register(lifetime.StopApplication);

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
}
