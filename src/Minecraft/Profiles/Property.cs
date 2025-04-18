namespace Void.Minecraft.Profiles;

public record Property
{
    public Property(string name, string value, bool isSigned = false, string? signature = null)
    {
        Name = name;
        Value = value;
        IsSigned = isSigned || !string.IsNullOrWhiteSpace(signature);
        Signature = signature;
    }

    public string Name { get; init; }
    public string Value { get; init; }
    public bool IsSigned { get; init; }
    public string? Signature { get; init; }
}
