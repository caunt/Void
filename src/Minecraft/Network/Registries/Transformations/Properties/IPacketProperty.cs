using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents an encoded packet field that can write itself to a Minecraft buffer.
/// </summary>
public interface IPacketProperty
{
    /// <summary>
    /// Writes the property's protocol representation at the current buffer position.
    /// </summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer);

    /// <summary>
    /// Casts this property to a requested packet-property type.
    /// </summary>
    /// <typeparam name="TCastValue">The property type expected by the caller.</typeparam>
    /// <returns>This instance cast to <typeparamref name="TCastValue" />.</returns>
    /// <exception cref="InvalidCastException">This instance is not a <typeparamref name="TCastValue" />.</exception>
    public virtual TCastValue As<TCastValue>() where TCastValue : IPacketProperty
    {
        if (this is not TCastValue value)
            throw new InvalidCastException($"Property value {this} cannot be cast to {typeof(TCastValue)}");

        return value;
    }
}

/// <summary>
/// Defines a packet property that can decode its own representation from a Minecraft buffer.
/// </summary>
/// <typeparam name="TPacketProperty">The concrete property type returned by decoding.</typeparam>
public interface IPacketProperty<TPacketProperty> : IPacketProperty where TPacketProperty : IPacketProperty<TPacketProperty>
{
    /// <summary>
    /// Reads a property from the current buffer position.
    /// </summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static abstract TPacketProperty Read(ref MinecraftBuffer buffer);
}
