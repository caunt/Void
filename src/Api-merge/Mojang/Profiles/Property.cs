namespace Void.Proxy.Api.Mojang.Profiles;

public record Property
{
    public Property(string Name, string Value, bool IsSigned, string? Signature)
    {
        this.Name = Name;
        this.Value = Value;
        this.IsSigned = IsSigned || !string.IsNullOrWhiteSpace(Signature);
        this.Signature = Signature;
    }

    public string Name { get; init; }
    public string Value { get; init; }
    public bool IsSigned { get; init; }
    public string? Signature { get; init; }

    public void Deconstruct(out string name, out string value, out bool isSigned, out string? signature)
    {
        name = Name;
        value = Value;
        isSigned = IsSigned;
        signature = Signature;
    }
}