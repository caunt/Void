using Extender;
using System.Collections;
using System.Reflection;
using Tomlet;
using Tomlet.Attributes;
using Tomlet.Exceptions;
using Tomlet.Models;
using Void.Proxy.Api.Configurations.Attributes;
using Void.Proxy.Api.Configurations.Exceptions;
using Void.Proxy.Api.Configurations.Serializer;

namespace Void.Proxy.Configurations.Serializers;

public class ConfigurationTomlSerializer : IConfigurationSerializer
{
    private readonly TomlParser _parser = new();
    private readonly TomlSerializerOptions _options = new()
    {
        // Allows record types constructors
        OverrideConstructorValues = true,
        IgnoreInvalidEnumValues = false,
        IgnoreNonPublicMembers = true
    };

    public string Serialize<TConfiguration>() where TConfiguration : notnull
    {
        return Serialize<TConfiguration>(default!);
    }

    public string Serialize<TConfiguration>(TConfiguration configuration) where TConfiguration : notnull
    {
        TomlDocument document;

        try
        {
            configuration ??= CreateInstanceWithDefaults<TConfiguration>();
            document = MapTomlDocument(configuration);
        }
        catch (TomlException exception)
        {
            throw new InvalidConfigurationException($"Failed to serialize configuration: {exception.Message}");
        }

        return document.SerializedValue;
    }

    public TConfiguration Deserialize<TConfiguration>(string source) where TConfiguration : notnull
    {
        TomlDocument document;

        try
        {
            document = _parser.Parse(source);
        }
        catch (TomlException exception)
        {
            throw new InvalidConfigurationException($"Failed to parse configuration file: {exception.Message}");
        }

        try
        {
            var configuration = TomletMain.To<TConfiguration>(document, _options);
            return ConfigurationService.CastConfiguration<TConfiguration>(configuration);
        }
        catch (TomlException exception)
        {
            throw new InvalidConfigurationException($"Failed to deserialize configuration: {exception.Message}");
        }
    }

    private static TConfiguration CreateInstanceWithDefaults<TConfiguration>() where TConfiguration : notnull
    {
        return (TConfiguration)CreateInstanceWithDefaults(typeof(TConfiguration));
    }

    private static object CreateInstanceWithDefaults(Type type, object? values = null)
    {
        // Handle arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType() ??
                throw new InvalidOperationException("Unable to determine array element type.");

            Array newArray;
            if (values is Array sourceArray)
            {
                newArray = Array.CreateInstance(elementType, sourceArray.Length);
                for (var i = 0; i < sourceArray.Length; i++)
                {
                    var element = sourceArray.GetValue(i);
                    newArray.SetValue(element switch
                    {
                        { } value when !IsSystemType(element.GetType()) => CreateInstanceWithDefaults(elementType, element),
                        _ => element
                    }, i);
                }
            }
            else
            {
                // No source array provided: create an empty array.
                newArray = Array.CreateInstance(elementType, 0);
            }

            return newArray;
        }

        // Handle generic List<T>
        if (IsList(type))
        {
            // If type is generic, retrieve the generic argument; otherwise use object.
            var elementType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);

            // If the provided type is an interface or abstract, create a concrete List<T> for the target element type.
            var concreteType = (type.IsInterface || type.IsAbstract) ? typeof(List<>).MakeGenericType(elementType) : type;

            var listInstance = (IList)(Activator.CreateInstance(concreteType)
                ?? throw new InvalidOperationException($"Unable to create instance of type {concreteType.FullName}"));

            if (values is IEnumerable sourceEnumerable)
            {
                foreach (var item in sourceEnumerable)
                {
                    listInstance.Add(item switch
                    {
                        { } value when !IsSystemType(item.GetType()) => CreateInstanceWithDefaults(elementType, item),
                        _ => item
                    });
                }
            }

            return listInstance;
        }

        // Handle generic Dictionary<K,V>
        if (IsDictionary(type))
        {
            Type keyType, valueType;

            // If the type is generic, extract the generic arguments.
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                keyType = genericArgs[0];
                valueType = genericArgs[1];
            }
            else // Fallback for non-generic IDictionary; use Object for key and value types.
            {
                keyType = typeof(object);
                valueType = typeof(object);
            }

            // For interfaces or abstract types, fallback to Dictionary<TKey, TValue>.
            var concreteType = (type.IsInterface || type.IsAbstract) ? typeof(Dictionary<,>).MakeGenericType(keyType, valueType) : type;

            var dictionaryInstance = (IDictionary)(Activator.CreateInstance(concreteType)
                ?? throw new InvalidOperationException($"Unable to create instance of type {concreteType.FullName}"));

            if (values is IDictionary sourceDictionary)
            {
                foreach (DictionaryEntry entry in sourceDictionary)
                {
                    var newKey = !IsSystemType(entry.Key.GetType())
                        ? CreateInstanceWithDefaults(keyType, entry.Key)
                        : entry.Key;

                    var newValue = entry.Value is not null && !IsSystemType(entry.Value.GetType())
                        ? CreateInstanceWithDefaults(valueType, entry.Value)
                        : entry.Value;

                    dictionaryInstance.Add(newKey, newValue);
                }
            }

            return dictionaryInstance;
        }

        // Handle generic IEnumerable<T> types not already covered
        if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();

            if (genericArgs.Length is 1) // Assuming a single-type enumerable
            {
                var elementType = genericArgs[0];
                object collectionInstance;

                // If type is interface or abstract, fallback to List<T>
                if (type.IsInterface || type.IsAbstract)
                {
                    collectionInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))
                        ?? throw new InvalidOperationException($"Unable to create instance of type {type.FullName}");
                }
                else
                {
                    // Try using a parameterless constructor first
                    var ctor = type.GetConstructor(Type.EmptyTypes);
                    collectionInstance = ctor is not null ? ctor.Invoke(null) : Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
                }

                // Find an "Add" method on the concrete type.
                var addMethod = collectionInstance.GetType().GetMethod("Add", [elementType]);

                if (addMethod is not null && values is IEnumerable sourceEnumerable)
                {
                    foreach (var item in sourceEnumerable)
                    {
                        var element = item != null && !IsSystemType(item.GetType())
                            ? CreateInstanceWithDefaults(elementType, item)
                            : item;

                        addMethod.Invoke(collectionInstance, [element]);
                    }

                    return collectionInstance;
                }
            }
        }

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

            var value = sourceProperty.GetValue(values);
            var valueType = value?.GetType();
            var valueInstance = value;

            if (value is not null && valueType is not null && (!IsSystemType(valueType) || IsDictionary(valueType) || IsList(valueType) || valueType.IsArray))
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

            if (value is not null && valueType is not null && (!IsSystemType(valueType) || IsDictionary(valueType) || IsList(valueType) || valueType.IsArray))
                valueInstance = CreateInstanceWithDefaults(field.FieldType, value);

            field.SetValue(instance, valueInstance);
        }

        return instance;

        static object CreateEmptyInstance(Type type)
        {
            if (type.IsInterface)
                throw new InvalidOperationException($"Cannot serialize interface {type.Name}");

            if (type.IsAbstract)
                throw new InvalidOperationException($"Cannot serialize serialize {type.Name}");

            // Assume the "primary" constructor is the one with the most parameters.
            // (Records typically have one primary constructor.)
            var constructor = type.GetConstructors().OrderByDescending(constructor => constructor.GetParameters().Length).FirstOrDefault()
                ?? throw new InvalidOperationException($"No public constructor found for type {type.Name}");

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

    private TomlDocument MapTomlDocument<TConfiguration>(TConfiguration configuration) where TConfiguration : notnull
    {
        var configurationType = typeof(TConfiguration);

        var extendedConfigurationType = MapTomlType(configurationType);
        var extendedConfiguration = CreateInstanceWithDefaults(extendedConfigurationType, configuration);

        var configurationDocument = TomletMain.DocumentFrom(configuration, _options);
        var extendedConfigurationDocument = TomletMain.DocumentFrom(extendedConfigurationType, extendedConfiguration, _options);

        SwapTomletConfiguration(configurationDocument, extendedConfigurationDocument);

        return configurationDocument;
    }

    private static Type MapTomlType(Type type)
    {
        if (type.IsArray)
        {
            var elementType = type.GetElementType() ?? throw new InvalidOperationException($"Array type {type} does not have an element type.");
            var mappedElementType = MapTomlType(elementType);

            return mappedElementType != elementType ? mappedElementType.MakeArrayType() : type;
        }

        if (type.IsGenericType)
        {
            var genericDefinition = type.GetGenericTypeDefinition();
            var genericArguments = type.GetGenericArguments();
            var mappedArguments = genericArguments.Select(MapTomlType).ToArray();

            // If any generic argument was mapped to a different type, rebuild the generic type.
            for (int i = 0; i < genericArguments.Length; i++)
            {
                if (genericArguments[i] == mappedArguments[i])
                    continue;

                return genericDefinition.MakeGenericType(mappedArguments);
            }

            return type;
        }

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

    private static bool IsList(Type type)
    {
        // Check if the type is a closed generic that is List<> or IList<>
        if (type.IsGenericType)
        {
            var gen = type.GetGenericTypeDefinition();

            if (gen == typeof(List<>) || gen == typeof(IList<>))
                return true;
        }

        // Also check if it implements the non-generic IList or any IList<> interface.
        return typeof(IList).IsAssignableFrom(type) || type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
    }

    private static bool IsDictionary(Type type)
    {
        // Check if the type itself is a closed generic Dictionary<,> or IDictionary<,>
        if (type.IsGenericType)
        {
            var gen = type.GetGenericTypeDefinition();

            if (gen == typeof(Dictionary<,>) || gen == typeof(IDictionary<,>))
                return true;
        }

        // Also check if it implements the non-generic IDictionary or any IDictionary<,> interface.
        return typeof(IDictionary).IsAssignableFrom(type) || type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
    }
}
