using Extender;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using Tomlet;
using Tomlet.Attributes;
using Tomlet.Exceptions;
using Tomlet.Models;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Configurations.Attributes;
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
        IgnoreNonPublicMembers = false
    };

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
            document = MapTomlDocument(configuration);
        }
        catch (TomlException exception)
        {
            throw new InvalidConfigurationException($"Failed to serialize configuration: {exception.Message}");
        }

        if (Path.GetDirectoryName(fileName) is { } path && !string.IsNullOrWhiteSpace(path))
            EnsureConfigurationsPathExists(path);

        await File.WriteAllTextAsync(fileName, document.SerializedValue, cancellationToken);
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

    private static TConfiguration CastConfiguration<TConfiguration>(object configuration) where TConfiguration : notnull
    {
        if (configuration is not TConfiguration castedConfiguration)
            throw new InvalidConfigurationException($"Found collision in configuration name: {configuration} != {typeof(TConfiguration)}");

        return castedConfiguration;
    }

    private static TConfiguration CreateInstanceWithDefaults<TConfiguration>()
    {
        return (TConfiguration)CreateInstanceWithDefaults(typeof(TConfiguration));
    }

    private static object CreateInstanceWithDefaults(Type type, object? values = null)
    {
        // If the type has an explicit parameterless constructor, use it.
        var parameterlessConstructor = type.GetConstructors().FirstOrDefault(constructor => constructor.GetParameters().Length == 0);

        var instance = parameterlessConstructor switch
        {
            { } value => value.Invoke(null),
            _ => CreateEmptyInstance(type)
        };

        // Map fields and properties from "object values" to instantiated type if provided
        var valuesType = values?.GetType();

        foreach (var property in type.GetProperties())
        {
            var sourceProperty = valuesType?.GetProperty(property.Name);

            if (sourceProperty is null)
                continue;

            if (!sourceProperty.CanRead)
                throw new InvalidOperationException($"Property {property.Name} is not readable in type {valuesType.FullName}");

            var value = sourceProperty.GetValue(values);
            var valueType = value?.GetType();
            var valueInstance = value;

            if (value is not null && valueType is not null && !IsSystemType(valueType))
                valueInstance = CreateInstanceWithDefaults(property.PropertyType, value);

            property.SetValue(instance, valueInstance);
        }

        foreach (var field in type.GetFields())
        {
            var sourceField = valuesType?.GetField(field.Name);

            if (sourceField is null)
                continue;

            var value = sourceField.GetValue(values);
            var valueType = value?.GetType();
            var valueInstance = value;

            if (value is not null && valueType is not null && !IsSystemType(valueType))
                valueInstance = CreateInstanceWithDefaults(field.FieldType, value);

            field.SetValue(instance, valueInstance);
        }

        return instance;

        static object CreateEmptyInstance(Type type)
        {
            // Assume the "primary" constructor is the one with the most parameters.
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
                if (parameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(parameter.ParameterType) is null)
                    return Activator.CreateInstance(parameter.ParameterType);

                // For reference types (or nullable value types), use null.
                return null;
            });

            return constructor.Invoke([.. args]);
        }
    }

    private void EnsureConfigurationsPathExists(string path = ConfigurationsPath)
    {
        if (Directory.Exists(path))
            return;

        logger.LogTrace("Creating {Path} directory", path);
        Directory.CreateDirectory(path);
    }

    private TomlDocument MapTomlDocument<TConfiguration>(TConfiguration configuration) where TConfiguration : notnull
    {
        var configurationType = typeof(TConfiguration);

        var extendedConfigurationType = MapTomlType(configurationType);
        var extendedConfiguration = CreateInstanceWithDefaults(extendedConfigurationType, configuration);

        var configurationDocument = TomletMain.DocumentFrom(configuration, _options);
        var extendedConfigurationDocument = TomletMain.DocumentFrom(extendedConfigurationType, extendedConfiguration, _options);

        System.Console.WriteLine(string.Join(", ", configurationType.GetProperties().Select(x => $"{x.Name} => {x.GetValue(configuration)}")));
        System.Console.WriteLine(string.Join(", ", extendedConfigurationType.GetProperties().Select(x => $"{x.Name} => {x.GetValue(extendedConfiguration)}")));
        SwapTomletConfiguration(configurationDocument, extendedConfigurationDocument);

        return configurationDocument;
    }

    private static Type MapTomlType(Type type)
    {
        if (IsSystemType(type))
            return type;

        var extender = new TypeExtender(type.Name + "TomletMapped");

        foreach (var attribute in type.GetCustomAttributes())
        {
            if (attribute is not ConfigurationAttribute configurationAttribute)
                continue;

            if (!string.IsNullOrWhiteSpace(configurationAttribute.InlineComment))
                extender.AddAttribute<TomlInlineCommentAttribute>([configurationAttribute.InlineComment]);

            if (!string.IsNullOrWhiteSpace(configurationAttribute.PrecedingComment))
                extender.AddAttribute<TomlPrecedingCommentAttribute>([configurationAttribute.PrecedingComment]);
        }

        foreach (var property in type.GetProperties())
        {
            var attributesWithValues = Enumerable.Empty<Tuple<Type, object[]>>();

            if (property.GetCustomAttribute<ConfigurationPropertyAttribute>() is { } configurationPropertyAttribute)
            {
                if (!string.IsNullOrWhiteSpace(configurationPropertyAttribute.InlineComment))
                    attributesWithValues = attributesWithValues.Append(Tuple.Create<Type, object[]>(typeof(TomlInlineCommentAttribute), [configurationPropertyAttribute.InlineComment]));

                if (!string.IsNullOrWhiteSpace(configurationPropertyAttribute.PrecedingComment))
                    attributesWithValues = attributesWithValues.Append(Tuple.Create<Type, object[]>(typeof(TomlPrecedingCommentAttribute), [configurationPropertyAttribute.PrecedingComment]));
            }

            extender.AddProperty(property.Name, MapTomlType(property.PropertyType), attributesWithValues);
        }

        foreach (var field in type.GetFields())
        {
            var attributesWithValues = Enumerable.Empty<Tuple<Type, object[]>>();

            if (field.GetCustomAttribute<ConfigurationPropertyAttribute>() is { } configurationPropertyAttribute)
            {
                if (!string.IsNullOrWhiteSpace(configurationPropertyAttribute.InlineComment))
                    attributesWithValues = attributesWithValues.Append(Tuple.Create<Type, object[]>(typeof(TomlInlineCommentAttribute), [configurationPropertyAttribute.InlineComment]));

                if (!string.IsNullOrWhiteSpace(configurationPropertyAttribute.PrecedingComment))
                    attributesWithValues = attributesWithValues.Append(Tuple.Create<Type, object[]>(typeof(TomlPrecedingCommentAttribute), [configurationPropertyAttribute.PrecedingComment]));
            }

            extender.AddField(field.Name, MapTomlType(field.FieldType), attributesWithValues.ToDictionary(x => x.Item1, x => x.Item2.ToList()));
        }

        return extender.FetchType();
    }

    private static void SwapTomletConfiguration(TomlTable table1, TomlTable table2)
    {
        table1.ForceNoInline = true;
        table2.ForceNoInline = true;

        foreach (var (key, value1) in table1)
        {
            if (!table2.TryGetValue(key, out var value2))
                throw new InvalidOperationException($"Key {key} not found in the second table.");

            var precedingComment1 = value1.Comments.PrecedingComment;
            var inlineComment1 = value1.Comments.InlineComment;

            value1.Comments.PrecedingComment = value2.Comments.PrecedingComment;
            value1.Comments.InlineComment = value2.Comments.InlineComment;

            value2.Comments.PrecedingComment = precedingComment1;
            value2.Comments.InlineComment = inlineComment1;

            if (value1 is TomlTable childTable1 && value2 is TomlTable childTable2)
                SwapTomletConfiguration(childTable1, childTable2);
        }
    }

    private static bool IsSystemType(Type type) => type.Namespace?.StartsWith("System") ?? true;
}
