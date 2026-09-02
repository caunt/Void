using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Registry;

/// <summary>Maps a command argument identifier to parser IDs that take effect at protocol-version thresholds.</summary>
/// <param name="Identifier">The namespaced parser identifier used before 1.19.</param>
/// <param name="VersionParserMappings">Parser-ID changes keyed by their first applicable protocol version.</param>
public record ArgumentSerializerMapping(string Identifier, Dictionary<ProtocolVersion, int> VersionParserMappings)
{
    private static readonly ProtocolVersion[] _protocolVersionsAscending = [.. ProtocolVersion.Range()];

    /// <summary>Gets the expanded parser ID for every supported applicable protocol version.</summary>
    public FrozenDictionary<ProtocolVersion, int> VersionParserIdMapping { get; } = Compute(VersionParserMappings);

    /// <summary>Creates an identifier-only mapping used by protocols before numeric parser IDs.</summary>
    /// <param name="identifier">The namespaced parser identifier.</param>
    public ArgumentSerializerMapping(string identifier) : this(identifier, [])
    {
        // Intentionally left blank.
    }

    /// <summary>Creates a mapping with one numeric parser-ID threshold.</summary>
    /// <param name="identifier">The namespaced parser identifier.</param>
    /// <param name="protocolVersion">The first version using the ID.</param>
    /// <param name="parserId">The numeric parser ID.</param>
    public ArgumentSerializerMapping(string identifier, ProtocolVersion protocolVersion, int parserId) : this(identifier, new() { [protocolVersion] = parserId })
    {
        // Intentionally left blank.
    }

    private static FrozenDictionary<ProtocolVersion, int> Compute(Dictionary<ProtocolVersion, int> versionId)
    {
        if (versionId.Count is 0)
            return FrozenDictionary<ProtocolVersion, int>.Empty;

        var thresholds = versionId.OrderBy(static pair => pair.Key).ToArray();
        var oldestVersion = thresholds[0].Key;

        if (oldestVersion < ProtocolVersion.MINECRAFT_1_19)
            throw new ArgumentException($"Version {oldestVersion} is too old for indexing", nameof(versionId));

        var mapping = new Dictionary<ProtocolVersion, int>(_protocolVersionsAscending.Length);

        var thresholdIndex = 0;
        var currentId = thresholds[0].Value;

        foreach (var protocolVersion in _protocolVersionsAscending)
        {
            if (protocolVersion < oldestVersion)
                continue;

            while (thresholdIndex + 1 < thresholds.Length && thresholds[thresholdIndex + 1].Key <= protocolVersion)
            {
                thresholdIndex++;
                currentId = thresholds[thresholdIndex].Value;
            }

            mapping[protocolVersion] = currentId;
        }

        return mapping.ToFrozenDictionary();
    }

    /// <summary>Attempts to get the numeric parser ID for an exact supported protocol version.</summary>
    /// <param name="version">The protocol version.</param>
    /// <param name="id">The mapped ID when present.</param>
    /// <returns><see langword="true"/> when a numeric mapping exists.</returns>
    public bool TryGetParserId(ProtocolVersion version, [MaybeNullWhen(false)] out int id)
    {
        if (!VersionParserIdMapping.TryGetValue(version, out id))
            return false;

        return id is not -1;
    }
}
