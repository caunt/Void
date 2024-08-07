﻿namespace Void.Proxy.Network.Protocol.Forge;

public class ForgeMarker
{
    private static readonly List<ForgeMarker> _markers = [];

    public static readonly ForgeMarker Forge = new("FORGE"); // 1.20.2
    public static readonly ForgeMarker Fml3 = new("FML3"); // 1.18.2
    public static readonly ForgeMarker Fml2 = new("FML2"); // 1.13
    public static readonly ForgeMarker Fml = new("FML"); // 1.7.2

    public ForgeMarker(string value)
    {
        Value = value;
        _markers.Add(this);
    }

    public static ForgeMarker? Longest => _markers.MaxBy(marker => marker.Value.Length);

    public string Value { get; init; }

    public static IEnumerable<ForgeMarker> Range()
    {
        return _markers.AsReadOnly();
    }
}