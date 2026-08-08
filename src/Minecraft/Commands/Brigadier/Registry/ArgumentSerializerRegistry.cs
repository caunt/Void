using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Definitions;

namespace Void.Minecraft.Commands.Brigadier.Registry;

/// <summary>Registers command argument serializers and resolves their protocol identifiers.</summary>
public class ArgumentSerializerRegistry
{
    private static readonly Dictionary<ArgumentSerializerMapping, IArgumentSerializer> MappingToSerializer = [];
    private static readonly Dictionary<Type, IArgumentSerializer> ArgumentTypeToSerializer = [];
    private static readonly Dictionary<Type, ArgumentSerializerMapping> ArgumentTypeToMapping = [];

    static ArgumentSerializerRegistry()
    {
        foreach (var definition in ArgumentParserDefinitions.BrigadierArgumentParserDefinitions)
            Register(definition.Mapping, definition.ArgumentType, definition.Serializer);

        foreach (var definition in ArgumentParserDefinitions.MinecraftArgumentParserDefinitions)
            Register(definition.Mapping, definition.ArgumentType, definition.Serializer);
    }

    /// <summary>Registers a passthrough mapping without associating a normal runtime argument type.</summary>
    /// <param name="mapping">The protocol identifier mapping.</param>
    /// <param name="serializer">The payload serializer, or the empty serializer when <see langword="null"/>.</param>
    public static void Register(ArgumentSerializerMapping mapping, IArgumentSerializer? serializer = null)
    {
        Register(mapping, argumentType: null, serializer ?? EmptyArgumentPassthroughSerializer.Instance);
    }

    /// <summary>Registers or replaces mapping and optional runtime-type associations.</summary>
    /// <param name="mapping">The protocol identifier mapping.</param>
    /// <param name="argumentType">The normal argument runtime type, or <see langword="null"/> for passthrough-only use.</param>
    /// <param name="serializer">The property serializer.</param>
    public static void Register(ArgumentSerializerMapping mapping, Type? argumentType, IArgumentSerializer serializer)
    {
        MappingToSerializer[mapping] = serializer;

        if (argumentType is null)
            return;

        ArgumentTypeToSerializer[argumentType] = serializer;
        ArgumentTypeToMapping[argumentType] = mapping;
    }

    /// <summary>Reads a parser identifier and its properties.</summary>
    /// <param name="buffer">The source command-tree buffer.</param>
    /// <param name="protocolVersion">The source protocol version.</param>
    /// <returns>A normal argument type or a wrapped passthrough argument.</returns>
    /// <exception cref="ArgumentException">The decoded mapping is not registered.</exception>
    public static IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var mapping = DecodeParserMapping(ref buffer, protocolVersion);

        if (!MappingToSerializer.TryGetValue(mapping, out var serializer))
            throw new ArgumentException($"Unexpected argument type mapping identifier {mapping.Identifier}.");

        var argumentType = serializer.Deserialize(ref buffer, protocolVersion);

        if (argumentType is IPassthroughArgumentValue passthroughArgumentValue)
            argumentType = new PassthroughArgumentType(mapping, passthroughArgumentValue);

        return argumentType;
    }

    /// <summary>Writes an argument parser identifier and its properties.</summary>
    /// <param name="buffer">The destination command-tree buffer.</param>
    /// <param name="argumentType">The normal or passthrough argument type.</param>
    /// <param name="protocolVersion">The target protocol version.</param>
    /// <exception cref="ArgumentException">The runtime argument type is unregistered or has no parser ID for the target version.</exception>
    public static void Serialize(ref BufferSpan buffer, IArgumentType argumentType, ProtocolVersion protocolVersion)
    {
        if (argumentType is PassthroughArgumentType passthroughArgumentType)
        {
            WriteParserIdentifier(ref buffer, passthroughArgumentType.Mappings, protocolVersion);
            passthroughArgumentType.Value.Serialize(ref buffer, protocolVersion);
            return;
        }

        var argumentTypeRuntimeType = argumentType.GetType();

        if (!ArgumentTypeToSerializer.TryGetValue(argumentTypeRuntimeType, out var serializer) || !ArgumentTypeToMapping.TryGetValue(argumentTypeRuntimeType, out var mapping))
            throw new ArgumentException($"Don't know how to serialize {argumentTypeRuntimeType.FullName}");

        WriteParserIdentifier(ref buffer, mapping, protocolVersion);
        serializer.Serialize(argumentType, ref buffer, protocolVersion);
    }

    /// <summary>Writes a numeric parser ID for 1.19 and newer, or a string identifier for older protocols.</summary>
    /// <param name="buffer">The destination buffer.</param>
    /// <param name="mapping">The parser mapping.</param>
    /// <param name="protocolVersion">The target protocol version.</param>
    /// <exception cref="ArgumentException">No numeric ID exists for a target version requiring one.</exception>
    public static void WriteParserIdentifier(ref BufferSpan buffer, ArgumentSerializerMapping mapping, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (!mapping.TryGetParserId(protocolVersion, out var parserId))
                throw new ArgumentException($"Argument type mapping {mapping} has no parser ID for protocol version {protocolVersion}.");

            buffer.WriteVarInt(parserId);
        }
        else
        {
            buffer.WriteString(mapping.Identifier);
        }
    }

    /// <summary>Reads and resolves a numeric or string parser identifier.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <param name="protocolVersion">The source protocol version.</param>
    /// <returns>The registered mapping.</returns>
    /// <exception cref="ArgumentException">The identifier is unknown for the protocol version.</exception>
    public static ArgumentSerializerMapping DecodeParserMapping(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            var protocolSpecificId = buffer.ReadVarInt();

            foreach (var mapping in MappingToSerializer.Keys)
            {
                if (!mapping.TryGetParserId(protocolVersion, out var parserId) || parserId != protocolSpecificId)
                    continue;

                return mapping;
            }

            throw new ArgumentException($"Argument type ID {protocolSpecificId} unknown for protocol version {protocolVersion}.");
        }
        else
        {
            var identifier = buffer.ReadString();

            foreach (var mapping in MappingToSerializer.Keys)
                if (mapping.Identifier == identifier)
                    return mapping;

            throw new ArgumentException($"Argument type mapping identifier {identifier} unknown.");
        }
    }
}
