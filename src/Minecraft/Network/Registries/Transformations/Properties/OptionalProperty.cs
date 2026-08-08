using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents a Boolean-presence-prefixed optional packet property.
/// </summary>
/// <typeparam name="TPacketProperty">The reference-type property decoded when the value is present.</typeparam>
/// <param name="Value">The optional value, or <see langword="null" /> for an absent property.</param>
public record OptionalProperty<TPacketProperty>(TPacketProperty? Value = null) : IPacketProperty<OptionalProperty<TPacketProperty>> where TPacketProperty : class, IPacketProperty<TPacketProperty>
{
    /// <summary>
    /// Reads the presence flag and, when set, one property value.
    /// </summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>An optional property containing the decoded value or <see langword="null" />.</returns>
    public static OptionalProperty<TPacketProperty> Read(ref MinecraftBuffer buffer)
    {
        var isPresent = buffer.ReadBoolean();

        if (!isPresent)
            return new();

        return new(TPacketProperty.Read(ref buffer));
    }

    /// <summary>
    /// Writes a presence flag and writes <see cref="Value" /> only when it is non-null.
    /// </summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteBoolean(Value is not null);
        Value?.Write(ref buffer);
    }
}
