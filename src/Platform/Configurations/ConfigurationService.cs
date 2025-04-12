using System.Reflection;
using System.Text;
using System.Threading.Channels;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Configurations.Exceptions;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Configurations;

public class ConfigurationService(ILogger<ConfigurationService> logger, IPluginService plugins) : BackgroundService, IConfigurationService
{
    private const string ConfigurationsPath = "configs";

    private readonly Dictionary<string, object> _configurations = [];
    private readonly TomlParser _parser = new();
    private readonly TomlSerializerOptions _options = new()
    {
        // Allows record types constructors
        OverrideConstructorValues = true,
        IgnoreInvalidEnumValues = false,
        IgnoreNonPublicMembers = true
    };

    public async ValueTask<TConfiguration> GetAsync<TConfiguration>(CancellationToken cancellationToken = default) where TConfiguration : notnull
    {
        var fileName = GetFileName<TConfiguration>();
        var configuration = await GetAsync<TConfiguration>(fileName, cancellationToken);

        return configuration;
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

        TomlDocument document;

        try
        {
            var source = await File.ReadAllTextAsync(fileName, cancellationToken);
            document = _parser.Parse(source);
        }
        catch (TomlException exception)
        {
            throw new InvalidConfigurationException($"Failed to parse configuration file: {exception.Message}");
        }

        try
        {
            configuration = TomletMain.To<TConfiguration>(document, _options);
            _configurations.Add(fileName, configuration);

            return CastConfiguration<TConfiguration>(configuration);
        }
        catch (TomlException exception)
        {
            throw new InvalidConfigurationException($"Failed to deserialize configuration: {exception.Message}");
        }
    }

    private async ValueTask SaveAsync<TConfiguration>(string fileName, CancellationToken cancellationToken) where TConfiguration : notnull
    {
        EnsureConfigurationsPathExists();

        TomlDocument document;

        try
        {
            var configuration = CreateInstanceWithDefaults<TConfiguration>();
            document = TomletMain.DocumentFrom(configuration);
        }
        catch (TomlException exception)
        {
            throw new InvalidConfigurationException($"Failed to serialize configuration: {exception.Message}");
        }

        document.ForceNoInline = true;

        if (Path.GetDirectoryName(fileName) is { } path && !string.IsNullOrWhiteSpace(path))
            EnsureConfigurationsPathExists(path);

        await File.WriteAllTextAsync(fileName, document.SerializedValue, cancellationToken);
    }

    private string GetFileName<TConfiguration>() where TConfiguration : notnull
    {
        var configurationType = typeof(TConfiguration);
        var configurationNameAttribute = configurationType.GetCustomAttribute<ConfigurationNameAttribute>();

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

    private static TConfiguration CastConfiguration<TConfiguration>(object configuration) where TConfiguration : notnull
    {
        if (configuration is not TConfiguration castedConfiguration)
            throw new InvalidConfigurationException($"Found collision in configuration name: {configuration} != {typeof(TConfiguration)}");

        return castedConfiguration;
    }

    private static TConfiguration CreateInstanceWithDefaults<TConfiguration>()
    {
        var type = typeof(TConfiguration);

        // If the type has an explicit parameterless constructor, use it.
        var parameterlessConstructor = type.GetConstructors().FirstOrDefault(constructor => constructor.GetParameters().Length == 0);

        if (parameterlessConstructor != null)
            return (TConfiguration)parameterlessConstructor.Invoke(null);

        // Otherwise, assume the "primary" constructor is the one with the most parameters.
        // (Records typically have one primary constructor.)
        var constructor = type.GetConstructors().OrderByDescending(constructor => constructor.GetParameters().Length).FirstOrDefault()
            ?? throw new InvalidOperationException($"No public constructor found for type {type.FullName}");

        // Prepare the arguments array by iterating over the parameters.
        var parameters = constructor.GetParameters();
        var args = parameters.Select(parameter =>
        {
            // If a default value is present, use it.
            if (parameter.HasDefaultValue)
                return parameter.DefaultValue;

            // For non-nullable value types, use their default instance.
            if (parameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(parameter.ParameterType) == null)
                return Activator.CreateInstance(parameter.ParameterType);

            // For reference types (or nullable value types), use null.
            return null;
        });

        return (TConfiguration)constructor.Invoke([.. args]);
    }

    private void EnsureConfigurationsPathExists(string path = ConfigurationsPath)
    {
        if (Directory.Exists(path))
            return;

        logger.LogTrace("Creating {Path} directory", path);
        Directory.CreateDirectory(path);
    }
}
