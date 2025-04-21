using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Void.Proxy")]
namespace Void.Proxy.Api.Extensions.Reflection;

public static class ObjectExtensions
{
    private const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

    internal static void InvokeMethod(this object instance, string methodName, params object[] args)
    {
        instance.InvokeMethod<object>(methodName, args);
    }

    internal static T? InvokeMethod<T>(this object instance, string methodName, params object[] args)
    {
        var method = instance.GetType().GetMethod(methodName, DefaultFlags) ??
            throw new MissingMethodException($"Method '{methodName}({string.Join(", ", args.Select(arg => arg.GetType()))})' not found in '{instance}'");

        return (T?)method.Invoke(instance, args);
    }

    internal static object? GetPropertyValue(this object instance, string propertyName)
    {
        return instance.GetPropertyValue<object>(propertyName);
    }

    internal static T? GetPropertyValue<T>(this object instance, string propertyName)
    {
        var type = instance.GetType();
        var propertyInfo = type.GetProperty(propertyName, DefaultFlags | BindingFlags.GetProperty);
        var value = default(T);

        if (propertyInfo != null)
        {
            value = (T?)propertyInfo.GetValue(instance, null);
        }
        else
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                propertyInfo = type.GetProperty(propertyName, DefaultFlags | BindingFlags.GetProperty);

                if (propertyInfo != null)
                {
                    value = (T?)propertyInfo.GetValue(instance, null);
                    break;
                }

                baseType = baseType.BaseType;
            }
        }

        return value;
    }

    internal static object? GetFieldValue(this object instance, string fieldName)
    {
        return instance.GetFieldValue<object>(fieldName);
    }

    internal static T? GetFieldValue<T>(this object instance, string fieldName)
    {
        var type = instance.GetType();
        var fieldInfo = type.GetField(fieldName, DefaultFlags | BindingFlags.GetField);
        var value = default(T);

        if (fieldInfo != null)
        {
            value = (T?)fieldInfo.GetValue(instance);
        }
        else
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                fieldInfo = baseType.GetField(fieldName, DefaultFlags | BindingFlags.GetField);

                if (fieldInfo != null)
                {
                    value = (T?)fieldInfo.GetValue(instance);
                    break;
                }

                baseType = baseType.BaseType;
            }
        }

        return value;
    }

    internal static void SetFieldValue(this object instance, string fieldName, object value)
    {
        var type = instance.GetType();
        var field = type.GetField(fieldName, DefaultFlags) ??
            throw new MissingFieldException($"Field '{fieldName}' not found in '{instance}'.");

        field.SetValue(instance, value);
    }
}
