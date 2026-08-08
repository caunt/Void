namespace Void.Minecraft.Profiles;

/// <summary>
/// Represents a named Minecraft profile property and its optional signature.
/// </summary>
public record Property
{
    /// <summary>
    /// Initializes a profile property.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The encoded property value.</param>
    /// <param name="isSigned">Whether the property should be marked as signed when no signature text is supplied.</param>
    /// <param name="signature">The optional cryptographic signature. A nonblank signature always marks the property as signed.</param>
    public Property(string name, string value, bool isSigned = false, string? signature = null)
    {
        Name = name;
        Value = value;
        IsSigned = isSigned || !string.IsNullOrWhiteSpace(signature);
        Signature = signature;
    }

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the encoded property value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Gets whether the property is marked as signed.
    /// </summary>
    public bool IsSigned { get; init; }

    /// <summary>
    /// Gets the optional property signature.
    /// </summary>
    /// <value>The signature text, or <see langword="null" /> when none was supplied.</value>
    public string? Signature { get; init; }
}
