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

    public static void Register(ArgumentSerializerMapping mapping, IArgumentSerializer? serializer = null)
    {
        Register(mapping, argumentType: null, serializer ?? EmptyArgumentPassthroughSerializer.Instance);
    }

    public static void Register(ArgumentSerializerMapping mapping, Type? argumentType, IArgumentSerializer serializer)
    {
        MappingToSerializer[mapping] = serializer;

        if (argumentType is null)
            return;

        ArgumentTypeToSerializer[argumentType] = serializer;
        ArgumentTypeToMapping[argumentType] = mapping;
    }

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
