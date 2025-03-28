using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Nbt.Serializers.Json;

public class NbtTagNumberAdapter
{
    private static readonly Dictionary<int, NbtTagType> _map = new()
    {
        [1] = NbtTagType.Byte,
        [2] = NbtTagType.Short,
        [3] = NbtTagType.Int,
        [4] = NbtTagType.Long,
        [5] = NbtTagType.Float,
        [6] = NbtTagType.Double,
    };

    public static NbtTagType GetTagsType(List<NbtTag> tags)
    {
        var types = tags.Select(tag => tag.Type).Distinct();

        if (types.Count() is 1)
            return types.ElementAt(0);

        throw new JsonException($"{nameof(NbtList)} cannot contain multiple NBT types. Present types: {string.Join(", ", types)}.");
    }

    public static void AlignNumbers(List<NbtTag> tags)
    {
        var maxRank = tags.Max(tag => GetTagRankFromType(tag.Type));
        var targetType = GetTagTypeFromRank(maxRank);
        for (var i = 0; i < tags.Count; i++)
        {
            var tag = tags[i];
            var rank = GetTagRankFromType(tag.Type);

            if (rank > 0 && rank < maxRank)
                tags[i] = ConvertTag(tag, targetType, GetTagValue(tag));
        }
    }

    private static NbtTag ConvertTag(NbtTag tag, NbtTagType? targetType, double value) => targetType switch
    {
        NbtTagType.Byte => new NbtByte((byte)value),
        NbtTagType.Short => new NbtShort((short)value),
        NbtTagType.Int => new NbtInt((int)value),
        NbtTagType.Long => new NbtLong((long)value),
        NbtTagType.Float => new NbtFloat((float)value),
        NbtTagType.Double => new NbtDouble(value),
        _ => tag
    };

    private static int GetTagRankFromType(NbtTagType type)
    {
        if (!_map.ContainsValue(type))
            return 0;

        var (rank, _) = _map.First(pair => pair.Value == type);
        return rank;
    }

    private static NbtTagType? GetTagTypeFromRank(int rank)
    {
        if (!_map.TryGetValue(rank, out var type))
            return null;

        return type;
    }

    private static double GetTagValue(NbtTag source) => source switch
    {
        NbtByte tag => tag.Value,
        NbtShort tag => tag.Value,
        NbtInt tag => tag.Value,
        NbtLong tag => tag.Value,
        NbtFloat tag => tag.Value,
        NbtDouble tag => tag.Value,
        _ => 0
    };
}