namespace Void.Proxy.API.Network.Protocol;

public class PacketMapping(
    int id,
    bool encodeOnly,
    ProtocolVersion protocolVersion,
    ProtocolVersion? lastValidProtocolVersion = null)
{
    public int Id { get; } = id;
    public bool EncodeOnly { get; } = encodeOnly;
    public ProtocolVersion ProtocolVersion { get; } = protocolVersion;
    public ProtocolVersion? LastValidProtocolVersion { get; } = lastValidProtocolVersion;

    public override bool Equals(object? obj)
    {
        if (this == obj)
            return true;

        if (obj == null || GetType() != obj.GetType())
            return false;

        var right = (PacketMapping)obj;
        return Id == right.Id && ProtocolVersion == right.ProtocolVersion && EncodeOnly == right.EncodeOnly;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ProtocolVersion, EncodeOnly);
    }
}