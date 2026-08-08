using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Minecraft.Network.Registries.Transformations.Mappings;

/// <summary>
/// Provides ordered, typed access to packet properties while a binary packet transformation is running.
/// </summary>
/// <remarks>
/// <para><see cref="Read{TPropertyValue}" /> consumes input without emitting it, <see cref="Write{TPropertyValue}(TPropertyValue)" /> appends output, and <see cref="Passthrough{TPropertyValue}" /> performs both operations.</para>
/// <para>Indexed access operates on properties already appended for output and counts occurrences of the requested type rather than absolute field positions.</para>
/// </remarks>
public interface IMinecraftBinaryPacketWrapper
{
    /// <summary>Attempts to get an emitted property by its one-based occurrence among properties of the requested type.</summary>
    /// <typeparam name="TPropertyValue">The property type to locate.</typeparam>
    /// <param name="index">The one-based occurrence index. The built-in implementation also treats nonpositive values as the first occurrence.</param>
    /// <param name="value">When this method returns <see langword="true" />, the matching property; otherwise, the default value.</param>
    /// <returns><see langword="true" /> when a matching emitted property exists; otherwise, <see langword="false" />.</returns>
    public bool TryGet<TPropertyValue>(int index, [MaybeNullWhen(false)] out TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;

    /// <summary>Gets an emitted property by its one-based occurrence among properties of the requested type.</summary>
    /// <typeparam name="TPropertyValue">The property type to locate.</typeparam>
    /// <param name="index">The one-based occurrence index. The built-in implementation also treats nonpositive values as the first occurrence.</param>
    /// <returns>The matching emitted property.</returns>
    /// <exception cref="System.InvalidOperationException">No matching property exists.</exception>
    public TPropertyValue Get<TPropertyValue>(int index) where TPropertyValue : IPacketProperty<TPropertyValue>;

    /// <summary>Attempts to replace an emitted property by occurrence among properties of the same type.</summary>
    /// <typeparam name="TPropertyValue">The property type to replace.</typeparam>
    /// <param name="index">The one-based occurrence index. The built-in implementation also treats nonpositive values as the first occurrence.</param>
    /// <param name="value">The replacement property.</param>
    /// <returns><see langword="true" /> when a matching property was replaced; otherwise, <see langword="false" />.</returns>
    public bool TrySet<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;

    /// <summary>Replaces an emitted property by occurrence among properties of the same type.</summary>
    /// <typeparam name="TPropertyValue">The property type to replace.</typeparam>
    /// <param name="index">The one-based occurrence index. The built-in implementation also treats nonpositive values as the first occurrence.</param>
    /// <param name="value">The replacement property.</param>
    /// <exception cref="System.InvalidOperationException">No matching property exists.</exception>
    public void Set<TPropertyValue>(int index, TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;

    /// <summary>Consumes and decodes the next input property without appending it to output.</summary>
    /// <typeparam name="TPropertyValue">The property type to decode.</typeparam>
    /// <returns>The consumed property.</returns>
    public TPropertyValue Read<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>;

    /// <summary>Appends a property to transformed output without consuming input.</summary>
    /// <typeparam name="TPropertyValue">The property type.</typeparam>
    /// <param name="value">The property to append.</param>
    public void Write<TPropertyValue>(TPropertyValue value) where TPropertyValue : IPacketProperty<TPropertyValue>;

    /// <summary>Consumes the next input property and appends the same property to output.</summary>
    /// <typeparam name="TPropertyValue">The property type to decode.</typeparam>
    /// <returns>The property that was consumed and appended.</returns>
    public TPropertyValue Passthrough<TPropertyValue>() where TPropertyValue : IPacketProperty<TPropertyValue>;

    /// <summary>Writes queued processed properties, or copies the underlying message stream when no queued properties remain.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void WriteProcessedValues(ref MinecraftBuffer buffer);

    /// <summary>Moves emitted output back into the readable queue in emission order and clears the output list.</summary>
    public void Reset();
}
