namespace MinecraftProxy.Models.Minecraft.Profile;

public class Property(string name, string value, bool isSigned, string? signature)
{
    public string Name { get; } = name;
    public string Value { get; } = value;
    public bool IsSigned { get; } = isSigned || signature is not null;
    public string? Signature { get; } = signature;
}
