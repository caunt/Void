namespace Void.Proxy.API.Network.Protocol;

public class ProtocolVersion : IComparable
{
    private static readonly Dictionary<int, ProtocolVersion> Mapping = [];

    public static readonly ProtocolVersion MINECRAFT_1_7_2 = new(4, "1.7.2", "1.7.3", "1.7.4", "1.7.5");
    public static readonly ProtocolVersion MINECRAFT_1_7_6 = new(5, "1.7.6", "1.7.7", "1.7.8", "1.7.9", "1.7.10");
    public static readonly ProtocolVersion MINECRAFT_1_8 = new(47, "1.8", "1.8.1", "1.8.2", "1.8.3", "1.8.4", "1.8.5", "1.8.6", "1.8.7", "1.8.8", "1.8.9");
    public static readonly ProtocolVersion MINECRAFT_1_9 = new(107, "1.9");
    public static readonly ProtocolVersion MINECRAFT_1_9_1 = new(108, "1.9.1");
    public static readonly ProtocolVersion MINECRAFT_1_9_2 = new(109, "1.9.2");
    public static readonly ProtocolVersion MINECRAFT_1_9_4 = new(110, "1.9.3", "1.9.4");
    public static readonly ProtocolVersion MINECRAFT_1_10 = new(210, "1.10", "1.10.1", "1.10.2");
    public static readonly ProtocolVersion MINECRAFT_1_11 = new(315, "1.11");
    public static readonly ProtocolVersion MINECRAFT_1_11_1 = new(316, "1.11.1", "1.11.2");
    public static readonly ProtocolVersion MINECRAFT_1_12 = new(335, "1.12");
    public static readonly ProtocolVersion MINECRAFT_1_12_1 = new(338, "1.12.1");
    public static readonly ProtocolVersion MINECRAFT_1_12_2 = new(340, "1.12.2");
    public static readonly ProtocolVersion MINECRAFT_1_13 = new(393, "1.13");
    public static readonly ProtocolVersion MINECRAFT_1_13_1 = new(401, "1.13.1");
    public static readonly ProtocolVersion MINECRAFT_1_13_2 = new(404, "1.13.2");
    public static readonly ProtocolVersion MINECRAFT_1_14 = new(477, "1.14");
    public static readonly ProtocolVersion MINECRAFT_1_14_1 = new(480, "1.14.1");
    public static readonly ProtocolVersion MINECRAFT_1_14_2 = new(485, "1.14.2");
    public static readonly ProtocolVersion MINECRAFT_1_14_3 = new(490, "1.14.3");
    public static readonly ProtocolVersion MINECRAFT_1_14_4 = new(498, "1.14.4");
    public static readonly ProtocolVersion MINECRAFT_1_15 = new(573, "1.15");
    public static readonly ProtocolVersion MINECRAFT_1_15_1 = new(575, "1.15.1");
    public static readonly ProtocolVersion MINECRAFT_1_15_2 = new(578, "1.15.2");
    public static readonly ProtocolVersion MINECRAFT_1_16 = new(735, "1.16");
    public static readonly ProtocolVersion MINECRAFT_1_16_1 = new(736, "1.16.1");
    public static readonly ProtocolVersion MINECRAFT_1_16_2 = new(751, "1.16.2");
    public static readonly ProtocolVersion MINECRAFT_1_16_3 = new(753, "1.16.3");
    public static readonly ProtocolVersion MINECRAFT_1_16_4 = new(754, "1.16.4", "1.16.5");
    public static readonly ProtocolVersion MINECRAFT_1_17 = new(755, "1.17");
    public static readonly ProtocolVersion MINECRAFT_1_17_1 = new(756, "1.17.1");
    public static readonly ProtocolVersion MINECRAFT_1_18 = new(757, "1.18", "1.18.1");
    public static readonly ProtocolVersion MINECRAFT_1_18_2 = new(758, "1.18.2");
    public static readonly ProtocolVersion MINECRAFT_1_19 = new(759, "1.19");
    public static readonly ProtocolVersion MINECRAFT_1_19_1 = new(760, "1.19.1", "1.19.2");
    public static readonly ProtocolVersion MINECRAFT_1_19_3 = new(761, "1.19.3");
    public static readonly ProtocolVersion MINECRAFT_1_19_4 = new(762, "1.19.4");
    public static readonly ProtocolVersion MINECRAFT_1_20 = new(763, "1.20", "1.20.1");
    public static readonly ProtocolVersion MINECRAFT_1_20_2 = new(764, "1.20.2");
    public static readonly ProtocolVersion MINECRAFT_1_20_3 = new(765, "1.20.3", "1.20.4");
    public static readonly ProtocolVersion MINECRAFT_1_20_5 = new(766, "1.20.5", "1.20.6");
    public static readonly ProtocolVersion MINECRAFT_1_21 = new(767, "1.21");

    public ProtocolVersion(int version, params string[] names)
    {
        Version = version;
        Names = names;

        if (!Mapping.TryAdd(version, this))
            throw new InvalidOperationException($"ProtocolVersion {version} already registered, use Search(<version>) instead");
    }

    public static ProtocolVersion Latest => Mapping.MaxBy(kv => kv.Key).Value;

    public static ProtocolVersion Oldest => Mapping.MinBy(kv => kv.Key).Value;

    public int Version { get; }
    public string[] Names { get; }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            null => 1,
            ProtocolVersion otherVersion => CompareTo(otherVersion),
            _ => throw new ArgumentException($"Object is not a {nameof(ProtocolVersion)}")
        };
    }

    public static ProtocolVersion Get(int version)
    {
        return Mapping[version];
    }

    public static IEnumerable<ProtocolVersion> Range()
    {
        return Range(Oldest, Latest);
    }

    public static IEnumerable<ProtocolVersion> Range(ProtocolVersion start, ProtocolVersion end)
    {
        return Mapping.Where(pair => pair.Key >= start.Version && pair.Key <= end.Version).Select(pair => pair.Value).Order();
    }

    public string GetVersionIntroducedIn()
    {
        return Names[0];
    }

    public string GetMostRecentSupportedVersion()
    {
        return Names[^1];
    }

    public IEnumerable<string> GetVersionsSupportedBy()
    {
        return Names.AsReadOnly();
    }

    public int CompareTo(ProtocolVersion? other)
    {
        return other is null ? 1 : Version.CompareTo(other.Version); // null is considered greater than non-null
    }

    public static bool operator >(ProtocolVersion left, ProtocolVersion right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <(ProtocolVersion left, ProtocolVersion right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >=(ProtocolVersion left, ProtocolVersion right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static bool operator <=(ProtocolVersion left, ProtocolVersion right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator ==(ProtocolVersion? left, ProtocolVersion? right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(ProtocolVersion? left, ProtocolVersion? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ProtocolVersion version)
            return false;

        return Version == version.Version;
    }

    public override int GetHashCode()
    {
        return Version.GetHashCode();
    }

    public override string ToString()
    {
        return GetVersionIntroducedIn();
    }
}