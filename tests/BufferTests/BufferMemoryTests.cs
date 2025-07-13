using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Exceptions;
using Xunit;

namespace Void.Tests.BufferTests;

public class BufferMemoryTests
{
    [Fact]
    public void Slice_ReturnsExpectedMemory()
    {
        var memory = new BufferMemory(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        var slice = memory.Slice(2, 5);
        Assert.Equal(new byte[] { 2, 3, 4, 5, 6 }, slice.Span.Access(0, 5).ToArray());
    }

    [Fact]
    public void Slice_WholeBuffer_ReturnsIdenticalBytes()
    {
        var data = new byte[] { 1, 2, 3 };
        var memory = new BufferMemory(data);

        var slice = memory.Slice(0, data.Length);

        Assert.Equal(data, slice.Span.Access(0, data.Length).ToArray());
    }

    [Fact]
    public void Slice_Modifications_AffectSource()
    {
        var data = new byte[] { 1, 2, 3, 4 };
        var memory = new BufferMemory(data);

        var slice = memory.Slice(1, 2);
        var span = slice.Span.Access(0, 2);
        span[0] = 9;

        Assert.Equal(9, memory.Span.Access(1, 2)[0]);
    }

    [Fact]
    public void Slice_ZeroLength_ReturnsEmptySpan()
    {
        var memory = new BufferMemory(new byte[] { 1, 2 });

        var slice = memory.Slice(1, 0);

        Assert.Equal(0, slice.Span.Length);
    }

    [Fact]
    public void Slice_NegativePosition_Throws()
    {
        var memory = new BufferMemory(new byte[10]);
        Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(-1, 1));
    }

    [Fact]
    public void Slice_NegativeLength_Throws()
    {
        var memory = new BufferMemory(new byte[10]);
        Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(0, -1));
    }

    [Fact]
    public void Slice_PastEnd_Throws()
    {
        var memory = new BufferMemory(new byte[5]);
        Assert.Throws<EndOfBufferException>(() => memory.Slice(3, 3));
    }

    [Fact]
    public void Span_ReturnsBufferSpanWithSameLength()
    {
        var memory = new BufferMemory(new byte[7]);
        Assert.Equal(7, memory.Span.Length);
    }
}
