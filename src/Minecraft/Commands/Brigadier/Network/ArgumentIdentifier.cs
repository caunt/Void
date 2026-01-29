using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Void.Minecraft.Network;
using static Void.Minecraft.Commands.Brigadier.Network.ArgumentIdentifier;

namespace Void.Minecraft.Commands.Brigadier.Network;

public record ArgumentIdentifier(string Identifier, params VersionSet[] Versions)
{
    public ImmutableDictionary<ProtocolVersion, int> VersionById { get; } = Compute(Versions);

    private static ImmutableDictionary<ProtocolVersion, int> Compute(VersionSet[] versions)
    {
        var temporaryVersionById = new Dictionary<ProtocolVersion, int>();

        ProtocolVersion? previousProtocolVersion = null;

        for (var versionSetIndex = 0; versionSetIndex < versions.Length; versionSetIndex++)
        {
            var currentVersionSet = versions[versionSetIndex] ?? throw new ArgumentNullException(nameof(versions), $"versions[{versionSetIndex}] is null.");

            if (currentVersionSet.Version < ProtocolVersion.MINECRAFT_1_19)
                throw new ArgumentException("Version too old for ID index", nameof(versions));

            if (previousProtocolVersion is not null && previousProtocolVersion <= currentVersionSet.Version)
                throw new ArgumentException("Invalid protocol version order", nameof(versions));

            foreach (var protocolVersion in ProtocolVersion.Range())
                if (protocolVersion >= currentVersionSet.Version)
                    temporaryVersionById.TryAdd(protocolVersion, currentVersionSet.Id);

            previousProtocolVersion = currentVersionSet.Version;
        }

        return temporaryVersionById.ToImmutableDictionary();
    }

    public int? GetIdByProtocolVersion(ProtocolVersion version)
    {
        if (VersionById.TryGetValue(version, out var identifierIndex))
            return identifierIndex;

        return null;
    }

    public static VersionSet MapSet(ProtocolVersion version, int id)
    {
        return new VersionSet(version, id);
    }

    public static ArgumentIdentifier Id(string identifier, params VersionSet[] versions)
    {
        return new ArgumentIdentifier(identifier, versions);
    }

    public record VersionSet(ProtocolVersion Version, int Id);
}
