using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

public static class PropertyTypes
{
    public static BoolType Bool { get; } = new();
    public static ByteType Byte { get; } = new();
    public static ShortType Short { get; } = new();
    public static IntType Int { get; } = new();
    public static LongType Long { get; } = new();
    public static FloatType Float { get; } = new();
    public static DoubleType Double { get; } = new();
    public static VarIntType VarInt { get; } = new();
    public static VarLongType VarLong { get; } = new();
    public static UuidType Uuid { get; } = new();
    public static NbtType Nbt { get; } = new();
}
