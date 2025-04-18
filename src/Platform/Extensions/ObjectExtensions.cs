using System.Reflection;

namespace Void.Proxy.Extensions;

public static class ObjectExtensions
{
    private const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

    public static void InvokeMethod(this object instance, string methodName, params object[] args)
    {
        instance.InvokeMethod<object>(methodName, args);
    }

    public static T? InvokeMethod<T>(this object instance, string methodName, params object[] args)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ??
            throw new MissingMethodException($"Method '{methodName}' not found in '{instance}'.");

        return (T?)method.Invoke(instance, args);
    }

    public static object? GetPropertyValue(this object instance, string propertyName)
    {
        return instance.GetPropertyValue<object>(propertyName);
    }

    public static T? GetPropertyValue<T>(this object instance, string propertyName)
    {
        var type = instance.GetType();
        var propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
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
                propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);

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

    public static object? GetFieldValue(this object instance, string fieldName)
    {
        return instance.GetFieldValue<object>(fieldName);
    }

    public static T? GetFieldValue<T>(this object instance, string fieldName)
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
}
