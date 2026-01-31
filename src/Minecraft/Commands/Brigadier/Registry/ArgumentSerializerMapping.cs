using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Registry;

public record ArgumentSerializerMapping(string Identifier, Dictionary<ProtocolVersion, int> VersionParserMappings)
{
    private static readonly ProtocolVersion[] _protocolVersionsAscending = [.. ProtocolVersion.Range()];

    public FrozenDictionary<ProtocolVersion, int> VersionParserIdMapping { get; } = Compute(VersionParserMappings);

    public ArgumentSerializerMapping(string identifier) : this(identifier, [])
    {
        // Intentionally left blank.
    }

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

    public bool TryGetParserId(ProtocolVersion version, [MaybeNullWhen(false)] out int id)
    {
        return VersionParserIdMapping.TryGetValue(version, out id);
    }
}
