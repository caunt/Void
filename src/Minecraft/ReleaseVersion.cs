using System;
using System.Diagnostics.CodeAnalysis;

namespace Void.Minecraft;

/// <summary>
/// Represents a Minecraft release version number in <c>Major.Minor[.Patch]</c> form.
/// </summary>
/// <remarks>
/// <para>
/// Patch is optional: when it is <c>0</c>, <see cref="ToString"/> omits it and returns the two-component form
/// (for example, <c>"1.21"</c>). A non-zero patch produces the three-component form (for example,
/// <c>"1.21.4"</c>).
/// </para>
/// <para>
/// Ordering follows the natural numeric ordering of the components: Major first, then Minor, then Patch.
/// </para>
/// <para>
/// Implicit conversions are provided in both directions so that a
/// <see cref="ReleaseVersion"/> can be assigned from or cast to a <see langword="string"/> without an explicit
/// call to <see cref="Parse"/>.
/// </para>
/// </remarks>
/// <param name="Major">The major component of the version number (for example, <c>1</c> in <c>1.21.4</c>).</param>
/// <param name="Minor">The minor component of the version number (for example, <c>21</c> in <c>1.21.4</c>).</param>
/// <param name="Patch">The patch component of the version number (for example, <c>4</c> in <c>1.21.4</c>). Pass <c>0</c> for releases that have no patch segment.</param>
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
