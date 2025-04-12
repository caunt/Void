using System.Reflection;
using System.Text;
using System.Threading.Channels;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Configurations.Attributes;
using Void.Proxy.Api.Configurations.Exceptions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Configurations.Serializers;

namespace Void.Proxy.Configurations;

public class ConfigurationService(ILogger<ConfigurationService> logger, IPluginService plugins) : BackgroundService, IConfigurationService
{
    private const string ConfigurationsPath = "configs";

    private readonly ConfigurationTomlSerializer _serializer = new();
    private readonly Dictionary<string, object> _configurations = [];

    public async ValueTask<TConfiguration> GetAsync<TConfiguration>(CancellationToken cancellationToken = default) where TConfiguration : notnull
    {
        var configurationType = typeof(TConfiguration);

        // Tomlet constraint
        if (configurationType.Attributes.HasFlag(TypeAttributes.Sealed) || (!configurationType.Attributes.HasFlag(TypeAttributes.Public) && !configurationType.Attributes.HasFlag(TypeAttributes.NestedPublic)))
            throw new ArgumentException($"{configurationType} is either sealed or not public");

        var fileName = GetFileName<TConfiguration>();
        var configuration = await GetAsync<TConfiguration>(fileName, cancellationToken);

        return configuration;
    }

    internal static TConfiguration CastConfiguration<TConfiguration>(object configuration) where TConfiguration : notnull
    {
        if (configuration is not TConfiguration castedConfiguration)
            throw new InvalidConfigurationException($"Found collision in configuration name: {configuration} != {typeof(TConfiguration)}");

        return castedConfiguration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        EnsureConfigurationsPathExists();

        var channel = Channel.CreateUnbounded<FileSystemEventArgs>();
        var watcher = new FileSystemWatcher
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Path = ConfigurationsPath,
            IncludeSubdirectories = true,
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

    private async ValueTask<TConfiguration> GetAsync<TConfiguration>(string fileName, CancellationToken cancellationToken) where TConfiguration : notnull
    {
        if (_configurations.TryGetValue(fileName, out var configuration))
            return CastConfiguration<TConfiguration>(configuration);

        logger.LogTrace("Loading configuration file {FileName}", fileName);

        if (!File.Exists(fileName))
            await SaveAsync<TConfiguration>(fileName, cancellationToken);

        var source = await File.ReadAllTextAsync(fileName, cancellationToken);
        configuration = _serializer.Deserialize<TConfiguration>(source);

        _configurations.Add(fileName, configuration);
        return CastConfiguration<TConfiguration>(configuration);
    }

    private async ValueTask SaveAsync<TConfiguration>(string fileName, CancellationToken cancellationToken) where TConfiguration : notnull
    {
        EnsureConfigurationsPathExists();

        if (Path.GetDirectoryName(fileName) is { } path && !string.IsNullOrWhiteSpace(path))
            EnsureConfigurationsPathExists(path);

        var value = _serializer.Serialize<TConfiguration>();
        await File.WriteAllTextAsync(fileName, value, cancellationToken);
    }

    private string GetFileName<TConfiguration>() where TConfiguration : notnull
    {
        var configurationType = typeof(TConfiguration);
        var configurationNameAttribute = configurationType.GetCustomAttribute<ConfigurationAttribute>();

        var pluginName = GetPluginNameFromConfiguration<TConfiguration>();
        var fileNameBuilder = new StringBuilder();

        fileNameBuilder.Append(ConfigurationsPath);
        fileNameBuilder.Append(Path.DirectorySeparatorChar);

        if (!string.IsNullOrWhiteSpace(pluginName))
        {
            fileNameBuilder.Append(pluginName);
            fileNameBuilder.Append(Path.DirectorySeparatorChar);
        }

        fileNameBuilder.Append(configurationNameAttribute switch
        {
            { Name: { } name } when !string.IsNullOrWhiteSpace(name) => name,
            _ => GetDefaultFileName()
        });

        fileNameBuilder.Append(".toml");

        return fileNameBuilder.ToString();

        static string GetDefaultFileName() => typeof(TConfiguration).Name;
    }

    private string? GetPluginNameFromConfiguration<TConfiguration>() where TConfiguration : notnull
    {
        var configurationType = typeof(TConfiguration);
        var assembly = configurationType.Assembly;
        var plugin = plugins.All.FirstOrDefault(plugin => plugin.GetType().Assembly == assembly);

        return plugin?.Name;
    }

    private void EnsureConfigurationsPathExists(string path = ConfigurationsPath)
    {
        if (Directory.Exists(path))
            return;

        logger.LogTrace("Creating {Path} directory", path);
        Directory.CreateDirectory(path);
    }
}
