using System.Threading.Channels;
using Void.Common.Plugins;
using Void.Proxy.Api.Configurations;
using IConfiguration = Void.Proxy.Api.Configurations.IConfiguration;

namespace Void.Proxy.Configurations;

public class ConfigurationService(ILogger<ConfigurationService> logger) : BackgroundService, IConfigurationService
{
    public TConfiguration Get<TConfiguration>(IPlugin plugin) where TConfiguration : IConfiguration
    {
        return default!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{Directory}", AppContext.BaseDirectory);

        var channel = Channel.CreateUnbounded<FileSystemEventArgs>();
        var watcher = new FileSystemWatcher
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Path = AppContext.BaseDirectory,
            EnableRaisingEvents = true
        };

        watcher.Created += (_, args) => channel.Writer.TryWrite(args);
        watcher.Deleted += (_, args) => channel.Writer.TryWrite(args);
        watcher.Renamed += (_, args) => channel.Writer.TryWrite(args);
        watcher.Changed += (_, args) => channel.Writer.TryWrite(args);

        await foreach (var change in channel.Reader.ReadAllAsync(stoppingToken))
        {
            // Handle the change event
        }
    }
}
