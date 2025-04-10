using System.Text.Json;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Adapter;

public class NbtTagBooleanAdapter
{
    public static NbtTag DeserializeBoolean(ref Utf8JsonReader reader) => reader.TokenType switch
    {
        JsonTokenType.True => new NbtByte(1),
        JsonTokenType.False => new NbtByte(0),
        _ => throw new JsonException($"\"{reader.GetString()}\" is not a boolean.")
    };
}
