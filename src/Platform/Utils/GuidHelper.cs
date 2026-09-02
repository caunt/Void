using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Utils;

public static class GuidHelper
{
    public static Guid FromStringHash(string text)
    {
        return Uuid.CreateVersion3(text);
    }

    public static Guid FromLongs(long mostSig, long leastSig)
    {
        return Uuid.FromLongs(mostSig, leastSig);
    }

    public static int GetVersion(Guid guid)
    {
        return guid.Version;
    }

    public static int GetVariant(Guid guid)
    {
        return ((Uuid)guid).Variant;
    }
}

public sealed class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetGuid();
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
