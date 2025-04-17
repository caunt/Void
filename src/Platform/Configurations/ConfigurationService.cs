using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Timers;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Configurations.Attributes;
using Void.Proxy.Api.Configurations.Exceptions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Extensions;
using Void.Proxy.Configurations.Serializers;
using Timer = System.Timers.Timer;

namespace Void.Proxy.Configurations;

public class ConfigurationService(ILogger<ConfigurationService> logger, IPluginService plugins) : BackgroundService, IConfigurationService
{
    private const string ConfigurationsPath = "configs";

    private readonly ConfigurationTomlSerializer _serializer = new();
    private readonly Dictionary<string, object> _configurations = [];

    [Subscribe(PostOrder.First)]
    public void OnPluginUnload(PluginUnloadEvent @event)
    {
        lock (this)
        {
            for (var i = _configurations.Count - 1; i >= 0; i--)
            {
                var (key, configuration) = _configurations.ElementAt(i);
                var configurationType = configuration.GetType();

                if (configurationType.Assembly != @event.Plugin.GetType().Assembly)
                    continue;

                _configurations.Remove(key);
            }
        }
    }

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

        var channel = Channel.CreateUnbounded<EventArgs>();
        var configurationWatcher = new Timer
        {
            Interval = 1000,
            AutoReset = true,
            Enabled = true
        };
        var fileSystemWatcher = new FileSystemWatcher
        {
            Filter = "*.*",
            NotifyFilter = NotifyFilters.LastWrite,
            Path = ConfigurationsPath,
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        configurationWatcher.Elapsed += (_, args) => channel.Writer.TryWrite(args);
        fileSystemWatcher.Created += (_, args) => channel.Writer.TryWrite(args);
        fileSystemWatcher.Deleted += (_, args) => channel.Writer.TryWrite(args);
        fileSystemWatcher.Renamed += (_, args) => channel.Writer.TryWrite(args);
        fileSystemWatcher.Changed += (_, args) => channel.Writer.TryWrite(args);

        var previousConfigurations = new Dictionary<string, string>();
        var skippedUpdates = new HashSet<string>();

        await foreach (var args in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                fileSystemWatcher.EnableRaisingEvents = false;

                switch (args)
                {
                    case FileSystemEventArgs fileSystemEventArgs:
                        {
                            if (fileSystemEventArgs.ChangeType is not WatcherChangeTypes.Changed)
                                continue;

                            // If changed directory, not a file, skip
                            if (!File.Exists(fileSystemEventArgs.FullPath))
                                continue;

                            if (!_configurations.TryGetValue(fileSystemEventArgs.FullPath, out var configuration))
                                continue;

                            if (skippedUpdates.Remove(fileSystemEventArgs.FullPath))
                                continue;

                            logger.LogInformation("Configuration {ConfigurationName} changed from disk", GetConfigurationName(configuration.GetType()));

                            var updatedConfiguration = await ReadAsync(fileSystemEventArgs.FullPath, configuration.GetType(), stoppingToken);
                            SwapConfiguration(configuration, updatedConfiguration);
                            break;
                        }
                    case ElapsedEventArgs:
                        {
                            foreach (var (key, configuration) in _configurations)
                            {
                                var serializedValue = _serializer.Serialize(configuration);

                                if (!previousConfigurations.TryGetValue(key, out var previousSerializedValue))
                                {
                                    previousConfigurations.Add(key, serializedValue);
                                    continue;
                                }

                                if (serializedValue != previousSerializedValue)
                                {
                                    logger.LogTrace("Configuration {ConfigurationName} changed", GetConfigurationName(configuration.GetType()));
                                    previousConfigurations[key] = serializedValue;

                                    await WaitFileLockAsync(key, stoppingToken);
                                    await SaveAsync(key, configuration, stoppingToken);

                                    // skippedUpdates.Add(key);
                                }
                            }
                            break;
                        }
                }
            }
            finally
            {
                fileSystemWatcher.EnableRaisingEvents = true;
            }
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

        await WaitFileLockAsync(fileName, cancellationToken);
        var source = await File.ReadAllTextAsync(fileName, cancellationToken);
        var configuration = _serializer.Deserialize(source, configurationType);

        return configuration;
    }

    private async ValueTask SaveAsync(string fileName, Type configurationType, CancellationToken cancellationToken)
    {
        logger.LogTrace("Saving default configuration file {FileName}", fileName);

        EnsureConfigurationsPathExists();

        if (Path.GetDirectoryName(fileName) is { } path && !string.IsNullOrWhiteSpace(path))
            EnsureConfigurationsPathExists(path);

        var value = _serializer.Serialize(configurationType);
        await WaitFileLockAsync(fileName, cancellationToken);
        await File.WriteAllTextAsync(fileName, value, cancellationToken);
    }

    private async ValueTask SaveAsync(string fileName, object configuration, CancellationToken cancellationToken)
    {
        logger.LogTrace("Saving configuration file {FileName}", fileName);

        EnsureConfigurationsPathExists();

        if (Path.GetDirectoryName(fileName) is { } path && !string.IsNullOrWhiteSpace(path))
            EnsureConfigurationsPathExists(path);

        var value = _serializer.Serialize(configuration);
        await WaitFileLockAsync(fileName, cancellationToken);
        await File.WriteAllTextAsync(fileName, value, cancellationToken);
    }

    private string GetFileName<TConfiguration>(string? key) where TConfiguration : notnull
    {
        var pluginName = GetPluginNameFromConfiguration<TConfiguration>();
        var fileNameBuilder = new StringBuilder();

        fileNameBuilder.Append(ConfigurationsPath);
        fileNameBuilder.Append(Path.DirectorySeparatorChar);

        if (!string.IsNullOrWhiteSpace(pluginName))
        {
            fileNameBuilder.Append(pluginName);
            fileNameBuilder.Append(Path.DirectorySeparatorChar);
        }

        fileNameBuilder.Append(GetConfigurationName(typeof(TConfiguration)));

        if (!string.IsNullOrWhiteSpace(key))
        {
            fileNameBuilder.Append(Path.DirectorySeparatorChar);
            fileNameBuilder.Append(key);
        }

        fileNameBuilder.Append(".toml");

        return fileNameBuilder.ToString();
    }

    private static string GetConfigurationName(Type configurationType)
    {
        return configurationType.GetCustomAttribute<ConfigurationAttribute>() switch
        {
            { Name: { } name } when !string.IsNullOrWhiteSpace(name) => name,
            _ => configurationType.Name
        };
    }

    private string? GetPluginNameFromConfiguration<TConfiguration>() where TConfiguration : notnull
    {
        plugins.TryGetPlugin(typeof(TConfiguration), out var plugin);
        return plugin?.Name;
    }

    private void EnsureConfigurationsPathExists(string path = ConfigurationsPath)
    {
        if (Directory.Exists(path))
            return;

        logger.LogTrace("Creating {Path} directory", path);
        Directory.CreateDirectory(path);
    }

    private static async ValueTask WaitFileLockAsync(string filePath, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!File.Exists(filePath))
                    break;

                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                break;
            }
            catch (IOException)
            {
                if (!File.Exists(filePath))
                    throw;
            }

            await Task.Delay(100, cancellationToken);
        }
    }

    private static void SwapConfiguration(object configuration1, object configuration2)
    {
        var configurationType1 = configuration1.GetType();
        var configurationType2 = configuration2.GetType();

        var fields1 = configurationType1.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        var fields2 = configurationType2.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        if (fields1.Length != fields2.Length)
            throw new InvalidOperationException($"Cannot swap configurations: {configurationType1} and {configurationType2} have different number of fields");

        for (var i = 0; i < fields1.Length; i++)
        {
            var field1 = fields1[i];
            var field2 = fields2[i];

            if (field1.Name != field2.Name)
                throw new InvalidOperationException($"Cannot swap configurations: {configurationType1} and {configurationType2} have different field names");

            if (field1.FieldType != field2.FieldType)
                throw new InvalidOperationException($"Cannot swap configurations: {configurationType1} and {configurationType2} have different field types");

            var field1Value = field1.GetValue(configuration1);
            var field2Value = field2.GetValue(configuration2);

            if (field1Value is null || field2Value is null || IsSystemType(field1Value) || IsSystemType(field2Value))
            {
                field1.SetValue(configuration1, field2Value);
                field2.SetValue(configuration2, field1Value);
            }
            else
            {
                SwapConfiguration(field1Value, field2Value);
            }
        }

        static bool IsSystemType(object value) => value.GetType().FullName?.StartsWith("System") ?? true;
    }
}
