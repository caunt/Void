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
        return await GetAsync<TConfiguration>(key: null, cancellationToken);
    }

    public async ValueTask<TConfiguration> GetAsync<TConfiguration>(string? key, CancellationToken cancellationToken = default) where TConfiguration : notnull
    {
        var configurationType = typeof(TConfiguration);

        // Tomlet constraint
        if (configurationType.Attributes.HasFlag(TypeAttributes.Sealed) || (!configurationType.Attributes.HasFlag(TypeAttributes.Public) && !configurationType.Attributes.HasFlag(TypeAttributes.NestedPublic)))
            throw new ArgumentException($"{configurationType} is either sealed or not public");

        var fileName = GetFileName<TConfiguration>(key);

        if (_configurations.TryGetValue(fileName, out var configuration))
            return CastConfiguration<TConfiguration>(configuration);

        _configurations.Add(fileName, await ReadAsync<TConfiguration>(fileName, cancellationToken));
        return await GetAsync<TConfiguration>(key, cancellationToken);
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
            Filter = "*.*",
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
            if (change.ChangeType is not WatcherChangeTypes.Changed)
                continue;

            // If changed directory, not a file, skip
            if (!File.Exists(change.FullPath))
                continue;

            if (!_configurations.TryGetValue(change.FullPath, out var configuration))
                continue;

            await WaitFileLockAsync(change.FullPath, stoppingToken);

            var updatedConfiguration = await ReadAsync(change.FullPath, configuration.GetType(), stoppingToken);
        }
    }

    private async ValueTask<TConfiguration> ReadAsync<TConfiguration>(string fileName, CancellationToken cancellationToken) where TConfiguration : notnull
    {
        return CastConfiguration<TConfiguration>(await ReadAsync(fileName, typeof(TConfiguration), cancellationToken));
    }

    private async ValueTask<object> ReadAsync(string fileName, Type configurationType, CancellationToken cancellationToken)
    {
        logger.LogTrace("Loading configuration file {FileName}", fileName);

        if (!File.Exists(fileName))
            await SaveAsync(fileName, configurationType, cancellationToken);

        var source = await File.ReadAllTextAsync(fileName, cancellationToken);
        var configuration = _serializer.Deserialize(source, configurationType);

        return configuration;
    }

    private async ValueTask SaveAsync(string fileName, Type configurationType, CancellationToken cancellationToken)
    {
        EnsureConfigurationsPathExists();

        if (Path.GetDirectoryName(fileName) is { } path && !string.IsNullOrWhiteSpace(path))
            EnsureConfigurationsPathExists(path);

        var value = _serializer.Serialize(configurationType);
        await File.WriteAllTextAsync(fileName, value, cancellationToken);
    }

    private string GetFileName<TConfiguration>(string? key) where TConfiguration : notnull
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

        if (!string.IsNullOrWhiteSpace(key))
        {
            fileNameBuilder.Append(Path.DirectorySeparatorChar);
            fileNameBuilder.Append(key);
        }

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

    private async ValueTask WaitFileLockAsync(string filePath, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return;
            }
            catch (IOException)
            {
                if (!File.Exists(filePath))
                    throw;
            }

            await Task.Delay(100, cancellationToken);
        }
    }
}
