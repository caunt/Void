using System.Text.Json;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json.Adapter;

/// <summary>Maps JSON boolean tokens to the conventional NBT byte values zero and one.</summary>
public class NbtTagBooleanAdapter
{
    /// <summary>Reads the current JSON boolean token as an NBT byte tag.</summary>
    /// <param name="reader">The reader positioned on a boolean token.</param>
    /// <returns>One for <see langword="true"/> or zero for <see langword="false"/>.</returns>
    /// <exception cref="JsonException">The current token is not a JSON boolean.</exception>
    public static NbtTag DeserializeBoolean(ref Utf8JsonReader reader) => reader.TokenType switch
    {
        JsonTokenType.True => new NbtByte(1),
        JsonTokenType.False => new NbtByte(0),
        _ => throw new JsonException($"{reader.TokenType} is not a boolean.")
    };
}
