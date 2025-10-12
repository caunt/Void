namespace Void.Proxy.Plugins.ModsSupport.Forge;

public class ForgeMarker
{
    private static readonly List<ForgeMarker> _markers = [];

    public static readonly ForgeMarker Fml = new("FML");
    public static readonly ForgeMarker Fml2 = new("FML2");
    public static readonly ForgeMarker Fml3 = new("FML3");
    public static readonly ForgeMarker Forge = new("FORGE");

    public static ForgeMarker Longest => _markers.MaxBy(marker => marker.Name.Length) ?? throw new InvalidOperationException("No Forge markers registered");

    public string Name { get; init; }

    public ForgeMarker(string value)
    {
        Name = value;
        _markers.Add(this);
    }

    public static IEnumerable<ForgeMarker> Range() => _markers.AsReadOnly();
}
