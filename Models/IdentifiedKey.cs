namespace MinecraftProxy.Models;

public class IdentifiedKey
{
    public long ExpiresAt { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] KeySignature { get; set; }
}