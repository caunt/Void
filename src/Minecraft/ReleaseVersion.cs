using System;
using System.Diagnostics.CodeAnalysis;

namespace Void.Minecraft;

public record ReleaseVersion(ushort Major, ushort Minor, ushort Patch) : IComparable<ReleaseVersion>
{
    public int CompareTo(ReleaseVersion? other)
    {
        return other is null ? 1 : (Major, Minor, Patch).CompareTo((other.Major, other.Minor, other.Patch));
    }

    public override string ToString()
    {
        return Patch is 0
            ? $"{Major}.{Minor}"
            : $"{Major}.{Minor}.{Patch}";
    }

    public static bool TryParse(string versionString, [MaybeNullWhen(false)] out ReleaseVersion version)
    {
        return (version = versionString.Split('.') switch
        {
            [var major, var minor] when ushort.TryParse(major, out var majorValue) && ushort.TryParse(minor, out var minorValue)
                => new ReleaseVersion(majorValue, minorValue, Patch: 0),
            [var major, var minor, var patch] when ushort.TryParse(major, out var majorValue) && ushort.TryParse(minor, out var minorValue) && ushort.TryParse(patch, out var patchValue)
                => new ReleaseVersion(majorValue, minorValue, patchValue),
            _ => null
        }) is not null;
    }

    public static ReleaseVersion Parse(string versionString)
    {
        return !TryParse(versionString, out var version)
            ? throw new FormatException($"Invalid version string: '{versionString}'.")
            : version;
    }

    public static implicit operator ReleaseVersion(string versionString)
    {
        return Parse(versionString);
    }

    public static implicit operator string(ReleaseVersion version)
    {
        return version.ToString();
    }
}
