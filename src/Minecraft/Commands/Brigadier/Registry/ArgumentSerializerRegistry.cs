using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Registry;

public class ArgumentSerializerRegistry
{
    private static readonly Dictionary<ArgumentSerializerMapping, IArgumentSerializer> MappingToSerializer = [];
    private static readonly Dictionary<Type, IArgumentSerializer> ArgumentTypeToSerializer = [];
    private static readonly Dictionary<Type, ArgumentSerializerMapping> ArgumentTypeToMapping = [];

    static ArgumentSerializerRegistry()
    {
        // Thanks to Velocity Contributors!
        // https://github.com/PaperMC/Velocity/

        // Base Brigadier argument types
        Register(new ArgumentSerializerMapping("brigadier:bool", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 0
        }), typeof(BoolArgumentType), BoolArgumentSerializer.Instance);

        Register(new ArgumentSerializerMapping("brigadier:float", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 1
        }), typeof(FloatArgumentType), FloatArgumentSerializer.Instance);
        Register(new ArgumentSerializerMapping("brigadier:double", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 2
        }), typeof(DoubleArgumentType), DoubleArgumentSerializer.Instance);
        Register(new ArgumentSerializerMapping("brigadier:integer", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 3
        }), typeof(IntegerArgumentType), IntegerArgumentSerializer.Instance);
        Register(new ArgumentSerializerMapping("brigadier:long", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 4
        }), typeof(LongArgumentType), LongArgumentSerializer.Instance);
        Register(new ArgumentSerializerMapping("brigadier:string", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 5
        }), typeof(StringArgumentType), StringArgumentSerializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:entity", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 6
        }), ByteArgumentPassthroughSerializer.Instance);
        Register(new ArgumentSerializerMapping("minecraft:game_profile", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 7
        }));
        Register(new ArgumentSerializerMapping("minecraft:block_pos", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 8
        }));
        Register(new ArgumentSerializerMapping("minecraft:column_pos", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 9
        }));
        Register(new ArgumentSerializerMapping("minecraft:vec3", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 10
        }));
        Register(new ArgumentSerializerMapping("minecraft:vec2", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 11
        }));
        Register(new ArgumentSerializerMapping("minecraft:block_state", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 12
        }));
        Register(new ArgumentSerializerMapping("minecraft:block_predicate", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 13
        }));
        Register(new ArgumentSerializerMapping("minecraft:item_stack", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 14
        }));
        Register(new ArgumentSerializerMapping("minecraft:item_predicate", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 15
        }));
        Register(new ArgumentSerializerMapping("minecraft:color", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = 16
        }));
        Register(new ArgumentSerializerMapping("minecraft:component", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 18,
            [ProtocolVersion.MINECRAFT_1_19] = 17
        }));
        Register(new ArgumentSerializerMapping("minecraft:style", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 19,
            [ProtocolVersion.MINECRAFT_1_20_3] = 18
        }));
        Register(new ArgumentSerializerMapping("minecraft:message", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 20,
            [ProtocolVersion.MINECRAFT_1_20_3] = 19,
            [ProtocolVersion.MINECRAFT_1_19] = 18
        }));
        Register(new ArgumentSerializerMapping("minecraft:nbt_compound_tag", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 21,
            [ProtocolVersion.MINECRAFT_1_20_3] = 20,
            [ProtocolVersion.MINECRAFT_1_19] = 19
        }));
        Register(new ArgumentSerializerMapping("minecraft:nbt_tag", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 22,
            [ProtocolVersion.MINECRAFT_1_20_3] = 21,
            [ProtocolVersion.MINECRAFT_1_19] = 20
        }));
        Register(new ArgumentSerializerMapping("minecraft:nbt_path", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 23,
            [ProtocolVersion.MINECRAFT_1_20_3] = 22,
            [ProtocolVersion.MINECRAFT_1_19] = 21
        }));
        Register(new ArgumentSerializerMapping("minecraft:objective", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 24,
            [ProtocolVersion.MINECRAFT_1_20_3] = 23,
            [ProtocolVersion.MINECRAFT_1_19] = 22
        }));
        Register(new ArgumentSerializerMapping("minecraft:objective_criteria", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 25,
            [ProtocolVersion.MINECRAFT_1_20_3] = 24,
            [ProtocolVersion.MINECRAFT_1_19] = 23
        }));
        Register(new ArgumentSerializerMapping("minecraft:operation", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 26,
            [ProtocolVersion.MINECRAFT_1_20_3] = 25,
            [ProtocolVersion.MINECRAFT_1_19] = 24
        }));
        Register(new ArgumentSerializerMapping("minecraft:particle", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 27,
            [ProtocolVersion.MINECRAFT_1_20_3] = 26,
            [ProtocolVersion.MINECRAFT_1_19] = 25
        }));
        Register(new ArgumentSerializerMapping("minecraft:angle", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 28,
            [ProtocolVersion.MINECRAFT_1_20_3] = 27,
            [ProtocolVersion.MINECRAFT_1_19] = 26
        }));
        Register(new ArgumentSerializerMapping("minecraft:rotation", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 29,
            [ProtocolVersion.MINECRAFT_1_20_3] = 28,
            [ProtocolVersion.MINECRAFT_1_19] = 27
        }));
        Register(new ArgumentSerializerMapping("minecraft:scoreboard_slot", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 30,
            [ProtocolVersion.MINECRAFT_1_20_3] = 29,
            [ProtocolVersion.MINECRAFT_1_19] = 28
        }));
        Register(new ArgumentSerializerMapping("minecraft:score_holder", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 31,
            [ProtocolVersion.MINECRAFT_1_20_3] = 30,
            [ProtocolVersion.MINECRAFT_1_19] = 29
        }), ByteArgumentPassthroughSerializer.Instance);
        Register(new ArgumentSerializerMapping("minecraft:swizzle", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 32,
            [ProtocolVersion.MINECRAFT_1_20_3] = 31,
            [ProtocolVersion.MINECRAFT_1_19] = 30
        }));
        Register(new ArgumentSerializerMapping("minecraft:team", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 33,
            [ProtocolVersion.MINECRAFT_1_20_3] = 32,
            [ProtocolVersion.MINECRAFT_1_19] = 31
        }));
        Register(new ArgumentSerializerMapping("minecraft:item_slot", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 34,
            [ProtocolVersion.MINECRAFT_1_20_3] = 33,
            [ProtocolVersion.MINECRAFT_1_19] = 32
        }));
        Register(new ArgumentSerializerMapping("minecraft:item_slots", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 35,
            [ProtocolVersion.MINECRAFT_1_20_5] = 34
        }));
        Register(new ArgumentSerializerMapping("minecraft:resource_location", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 36,
            [ProtocolVersion.MINECRAFT_1_20_5] = 35,
            [ProtocolVersion.MINECRAFT_1_20_3] = 34,
            [ProtocolVersion.MINECRAFT_1_19] = 33
        }));

        Register(new ArgumentSerializerMapping("minecraft:mob_effect", new()
        {
            [ProtocolVersion.MINECRAFT_1_19_3] = -1,
            [ProtocolVersion.MINECRAFT_1_19] = 34
        }));

        Register(new ArgumentSerializerMapping("minecraft:function", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 37,
            [ProtocolVersion.MINECRAFT_1_20_5] = 36,
            [ProtocolVersion.MINECRAFT_1_20_3] = 35,
            [ProtocolVersion.MINECRAFT_1_19_3] = 34,
            [ProtocolVersion.MINECRAFT_1_19] = 35
        }));

        Register(new ArgumentSerializerMapping("minecraft:entity_anchor", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 38,
            [ProtocolVersion.MINECRAFT_1_20_5] = 37,
            [ProtocolVersion.MINECRAFT_1_20_3] = 36,
            [ProtocolVersion.MINECRAFT_1_19_3] = 35,
            [ProtocolVersion.MINECRAFT_1_19] = 36
        }));

        Register(new ArgumentSerializerMapping("minecraft:int_range", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 39,
            [ProtocolVersion.MINECRAFT_1_20_5] = 38,
            [ProtocolVersion.MINECRAFT_1_20_3] = 37,
            [ProtocolVersion.MINECRAFT_1_19_3] = 36,
            [ProtocolVersion.MINECRAFT_1_19] = 37
        }));

        Register(new ArgumentSerializerMapping("minecraft:float_range", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 40,
            [ProtocolVersion.MINECRAFT_1_20_5] = 39,
            [ProtocolVersion.MINECRAFT_1_20_3] = 38,
            [ProtocolVersion.MINECRAFT_1_19_3] = 37,
            [ProtocolVersion.MINECRAFT_1_19] = 38
        }));

        Register(new ArgumentSerializerMapping("minecraft:item_enchantment", new()
        {
            [ProtocolVersion.MINECRAFT_1_19_3] = -1,
            [ProtocolVersion.MINECRAFT_1_19] = 39
        }));
        Register(new ArgumentSerializerMapping("minecraft:entity_summon", new()
        {
            [ProtocolVersion.MINECRAFT_1_19_3] = -1,
            [ProtocolVersion.MINECRAFT_1_19] = 40
        }));

        Register(new ArgumentSerializerMapping("minecraft:dimension", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 41,
            [ProtocolVersion.MINECRAFT_1_20_5] = 40,
            [ProtocolVersion.MINECRAFT_1_20_3] = 39,
            [ProtocolVersion.MINECRAFT_1_19_3] = 38,
            [ProtocolVersion.MINECRAFT_1_19] = 41
        }));

        Register(new ArgumentSerializerMapping("minecraft:gamemode", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 42,
            [ProtocolVersion.MINECRAFT_1_20_5] = 41,
            [ProtocolVersion.MINECRAFT_1_20_3] = 40,
            [ProtocolVersion.MINECRAFT_1_19_3] = 39
        }));

        Register(new ArgumentSerializerMapping("minecraft:time", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 43,
            [ProtocolVersion.MINECRAFT_1_20_5] = 42,
            [ProtocolVersion.MINECRAFT_1_20_3] = 41,
            [ProtocolVersion.MINECRAFT_1_19_3] = 40,
            [ProtocolVersion.MINECRAFT_1_19] = 42
        }), TimeArgumentPassthroughSerializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:resource_or_tag", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 44,
            [ProtocolVersion.MINECRAFT_1_20_5] = 43,
            [ProtocolVersion.MINECRAFT_1_20_3] = 42,
            [ProtocolVersion.MINECRAFT_1_19_3] = 41,
            [ProtocolVersion.MINECRAFT_1_19] = 43
        }), typeof(RegistryKeyArgumentType), RegistryKeyArgumentSerializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:resource_or_tag_key", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 45,
            [ProtocolVersion.MINECRAFT_1_20_5] = 44,
            [ProtocolVersion.MINECRAFT_1_20_3] = 43,
            [ProtocolVersion.MINECRAFT_1_19_3] = 42
        }), typeof(ResourceOrTagKeyArgumentType), ResourceOrTagKeyArgumentType.Serializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:resource", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 46,
            [ProtocolVersion.MINECRAFT_1_20_5] = 45,
            [ProtocolVersion.MINECRAFT_1_20_3] = 44,
            [ProtocolVersion.MINECRAFT_1_19_3] = 43,
            [ProtocolVersion.MINECRAFT_1_19] = 44
        }), typeof(RegistryKeyArgumentType), RegistryKeyArgumentSerializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:resource_key", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 47,
            [ProtocolVersion.MINECRAFT_1_20_5] = 46,
            [ProtocolVersion.MINECRAFT_1_20_3] = 45,
            [ProtocolVersion.MINECRAFT_1_19_3] = 44
        }), typeof(ResourceKeyArgumentType), ResourceKeyArgumentType.Serializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:resource_selector", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 48,
            [ProtocolVersion.MINECRAFT_1_21_5] = 47
        }), typeof(ResourceSelectorArgumentType), ResourceSelectorArgumentType.Serializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:template_mirror", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 49,
            [ProtocolVersion.MINECRAFT_1_21_5] = 48,
            [ProtocolVersion.MINECRAFT_1_20_5] = 47,
            [ProtocolVersion.MINECRAFT_1_20_3] = 46,
            [ProtocolVersion.MINECRAFT_1_19] = 45
        }));

        Register(new ArgumentSerializerMapping("minecraft:template_rotation", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 50,
            [ProtocolVersion.MINECRAFT_1_21_5] = 49,
            [ProtocolVersion.MINECRAFT_1_20_5] = 48,
            [ProtocolVersion.MINECRAFT_1_20_3] = 47,
            [ProtocolVersion.MINECRAFT_1_19] = 46
        }));

        Register(new ArgumentSerializerMapping("minecraft:heightmap", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 51,
            [ProtocolVersion.MINECRAFT_1_21_5] = 50,
            [ProtocolVersion.MINECRAFT_1_20_3] = 49,
            [ProtocolVersion.MINECRAFT_1_19_4] = 47
        }));

        Register(new ArgumentSerializerMapping("minecraft:uuid", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 56,
            [ProtocolVersion.MINECRAFT_1_21_5] = 54,
            [ProtocolVersion.MINECRAFT_1_20_5] = 53,
            [ProtocolVersion.MINECRAFT_1_20_3] = 48,
            [ProtocolVersion.MINECRAFT_1_19_4] = 48,
            [ProtocolVersion.MINECRAFT_1_19] = 47
        }));

        Register(new ArgumentSerializerMapping("minecraft:loot_table", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 52,
            [ProtocolVersion.MINECRAFT_1_21_5] = 51,
            [ProtocolVersion.MINECRAFT_1_20_5] = 50
        }));

        Register(new ArgumentSerializerMapping("minecraft:loot_predicate", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 53,
            [ProtocolVersion.MINECRAFT_1_21_5] = 52,
            [ProtocolVersion.MINECRAFT_1_20_5] = 51
        }));

        Register(new ArgumentSerializerMapping("minecraft:loot_modifier", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 54,
            [ProtocolVersion.MINECRAFT_1_21_5] = 53,
            [ProtocolVersion.MINECRAFT_1_20_5] = 52
        }));

        Register(new ArgumentSerializerMapping("minecraft:hex_color", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 17
        }));
        Register(new ArgumentSerializerMapping("minecraft:dialog", new()
        {
            [ProtocolVersion.MINECRAFT_1_21_6] = 55
        }));

        Register(new ArgumentSerializerMapping("crossstitch:mod_argument", new()
        {
            [ProtocolVersion.MINECRAFT_1_19] = -256
        }), typeof(CrossStitchModArgumentType), CrossStitchModArgumentSerializer.Instance);

        Register(new ArgumentSerializerMapping("minecraft:nbt"));
    }

    public static ArgumentBuilder DeserializeArgumentBuilder(ref BufferSpan buffer, string name, ProtocolVersion protocolVersion)
    {
        var mapping = DecodeParserMapping(ref buffer, protocolVersion);

        if (!MappingToSerializer.TryGetValue(mapping, out var serializer))
            throw new ArgumentException($"Unexpected argument type mapping identifier {mapping.Identifier}.");

        var argumentType = serializer.Deserialize(ref buffer, protocolVersion);
        var argumentBuilder = RequiredArgumentBuilder.Create(name, argumentType switch
        {
            IPassthroughArgumentValue passthroughArgumentValue => new PassthroughArgumentType(mapping, passthroughArgumentValue),
            _ => argumentType
        });

        return argumentBuilder;
    }

    public static void Serialize(ref BufferSpan buffer, ArgumentCommandNode argumentCommandNode, ProtocolVersion protocolVersion)
    {
        var argumentType = argumentCommandNode.Type;

        if (argumentType is PassthroughArgumentType passthroughArgumentType)
        {
            WriteParserMapping(ref buffer, passthroughArgumentType.Mappings, protocolVersion);
            passthroughArgumentType.Value.Serialize(ref buffer, protocolVersion);

            return;
        }

        if (argumentType is CrossStitchModArgumentType modArgumentProperty)
        {
            WriteParserMapping(ref buffer, modArgumentProperty.Mapping, protocolVersion);
            var modArgumentBuffer = modArgumentProperty.Data.Span;
            buffer.Write(modArgumentBuffer.ReadToEnd());
            return;
        }

        var argumentTypeRuntimeType = argumentType.GetType();

        if (!ArgumentTypeToSerializer.TryGetValue(argumentTypeRuntimeType, out var serializer) || !ArgumentTypeToMapping.TryGetValue(argumentTypeRuntimeType, out var mapping))
            throw new ArgumentException($"Don't know how to serialize {argumentTypeRuntimeType.FullName}");

        WriteParserMapping(ref buffer, mapping, protocolVersion);
        serializer.Serialize(argumentType, ref buffer, protocolVersion);
    }

    public static void WriteParserMapping(ref BufferSpan buffer, ArgumentSerializerMapping mapping, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (!mapping.TryGetParserId(protocolVersion, out var parserId))
                throw new ArgumentException($"Argument type mapping {mapping} has no parser ID for protocol version {protocolVersion}.");

            buffer.WriteVarInt(parserId);
            return;
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

    private static void Register(ArgumentSerializerMapping mapping, IArgumentSerializer? serializer = null)
    {
        Register(mapping, argumentType: null, serializer ?? EmptyArgumentPassthroughSerializer.Instance);
    }

    private static void Register(ArgumentSerializerMapping mapping, Type? argumentType, IArgumentSerializer serializer)
    {
        MappingToSerializer[mapping] = serializer;

        if (argumentType is null)
            return;

        ArgumentTypeToSerializer[argumentType] = serializer;
        ArgumentTypeToMapping[argumentType] = mapping;
    }
}
