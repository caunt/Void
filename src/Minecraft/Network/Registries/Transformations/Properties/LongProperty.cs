using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record LongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<LongProperty>
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadLong();

    public static LongProperty FromPrimitive(long value)
    {
        Span<byte> span = stackalloc byte[8];
        var buffer = new MinecraftBuffer(span);
        buffer.WriteLong(value);

        var array = GC.AllocateUninitializedArray<byte>(8);
        span.CopyTo(array);

        return new LongProperty(array);
    }
    public static LongProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadLong());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteLong(AsPrimitive);
    }
}
