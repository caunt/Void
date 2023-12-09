namespace MinecraftProxy.Network.Protocol.Forge;

public class ForgeMarker
{
    private static readonly List<ForgeMarker> _markers = [];

    public static readonly ForgeMarker Forge_1_20_4 = new("FORGE");
    public static ForgeMarker? Longest => _markers.MaxBy(marker => marker.Value.Length);

    public string Value { get; init; }

    public ForgeMarker(string value)
    {
        Value = value;
        _markers.Add(this);
    }

    public static IEnumerable<ForgeMarker> Range() => _markers.AsReadOnly();
}