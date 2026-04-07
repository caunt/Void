using System.Collections.Frozen;
using Void.Minecraft;
using Void.Minecraft.Network;

namespace Void.IntegrationTests.Generators;

public static class ReleaseVersionGroups
{
    public static readonly IReadOnlyDictionary<ReleaseVersion, ReleaseVersion[]> MajorMinor = ProtocolVersion
        .Range(ProtocolVersion.Oldest, ProtocolVersion.Latest)
        .SelectMany(version => version.Releases.Select(release => (MajorMinor: (release.Major, release.Minor), Version: version)))
        .ToLookup(item => item.MajorMinor, item => item.Version)
        .ToFrozenDictionary(group => group.First().FirstRelease, group => group.Select(version => version.FirstRelease).Distinct().ToArray());
}
