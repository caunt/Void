using System;
using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Exceptions;
using Xunit;

namespace Void.Tests.BufferTests;

public class BufferSpanTests
{
    [Fact]
    public void Position_SetValid_Succeeds()
    {
        var span = new BufferSpan(stackalloc byte[10]);
        span.Position = 5;
        Assert.Equal(5, span.Position);
    }

    [Fact]
    public void Position_SetNegative_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[10]);
            span.Position = -1;
        });
    }

    [Fact]
    public void Position_SetPastEnd_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[10]);
            span.Position = 11;
        });
    }

    [Fact]
    public void Length_ReturnsSourceLength()
    {
        var span = new BufferSpan(stackalloc byte[7]);

        Assert.Equal(7, span.Length);
    }

    [Fact]
    public void Remaining_UpdatesWithPosition()
    {
        var span = new BufferSpan(stackalloc byte[5]);

        Assert.Equal(5, span.Remaining);

        span.Position = 2;

        Assert.Equal(3, span.Remaining);
    }

    [Fact]
    public void Slice_ReturnsExpectedSpan()
    {
        Span<byte> source = stackalloc byte[] { 0, 1, 2, 3, 4 };
        var span = new BufferSpan(source);
        var slice = span.Slice(1, 3);
        Span<byte> expected = stackalloc byte[] { 1, 2, 3 };
        Assert.True(slice.Access(0, 3).SequenceEqual(expected));
    }

    [Fact]
    public void Slice_Modifications_AffectSource()
    {
        Span<byte> source = stackalloc byte[] { 0, 1, 2, 3 };
        var span = new BufferSpan(source);

        var slice = span.Slice(1, 2);
        slice.Access(0, 2)[0] = 9;

        Assert.Equal(9, span.Access(1, 2)[0]);
    }

    [Fact]
    public void Slice_ZeroLength_ReturnsEmptySpan()
    {
        Span<byte> source = stackalloc byte[] { 0, 1 };
        var span = new BufferSpan(source);

        var slice = span.Slice(1, 0);

        Assert.Equal(0, slice.Length);
    }

    [Fact]
    public void Access_ByLength_UsesCurrentPosition()
    {
        Span<byte> source = stackalloc byte[] { 0, 1, 2, 3, 4 };
        var span = new BufferSpan(source);
        span.Position = 2;
        Assert.Equal(new byte[] { 2, 3 }, span.Access(2).ToArray());
    }

    [Fact]
    public void Access_ZeroLength_ReturnsEmpty()
    {
        var span = new BufferSpan(stackalloc byte[5]);

        Assert.Equal(0, span.Access(0).Length);
    }

    [Fact]
    public void Access_WholeSpan_ReturnsAllBytes()
    {
        Span<byte> source = stackalloc byte[] { 1, 2, 3 };
        var span = new BufferSpan(source);

        Assert.Equal(new byte[] { 1, 2, 3 }, span.Access(0, 3).ToArray());
    }

    [Fact]
    public void Access_OutOfRange_Throws()
    {
        Assert.Throws<EndOfBufferException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[5]);
            span.Access(3, 3);
        });
    }

    [Fact]
    public void Access_NegativePosition_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[5]);
            span.Access(-1, 1);
        });
    }

    [Fact]
    public void Access_NegativeLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[5]);
            span.Access(0, -1);
        });
    }

    [Fact]
    public void Access_LengthTooLong_Throws()
    {
        Assert.Throws<EndOfBufferException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[5]);
            span.Access(4, 2);
        });
    }

    [Fact]
    public void Seek_Begin_UpdatesPosition()
    {
        var span = new BufferSpan(stackalloc byte[10]);
        span.Seek(3, SeekOrigin.Begin);
        Assert.Equal(3, span.Position);
    }

    [Fact]
    public void Seek_Current_UpdatesPosition()
    {
        var span = new BufferSpan(stackalloc byte[10]);
        span.Position = 2;
        span.Seek(3, SeekOrigin.Current);
        Assert.Equal(5, span.Position);
    }

    [Fact]
    public void Seek_ZeroOffset_DoesNotChangePosition()
    {
        var span = new BufferSpan(stackalloc byte[10]);
        span.Position = 4;

        span.Seek(0, SeekOrigin.Current);

        Assert.Equal(4, span.Position);
    }

    [Fact]
    public void Seek_EndWithZeroOffset_SetsToEnd()
    {
        var span = new BufferSpan(stackalloc byte[10]);

        span.Seek(0, SeekOrigin.End);

        Assert.Equal(10, span.Position);
    }

    [Fact]
    public void Seek_BeginWithZeroOffset_SetsToStart()
    {
        var span = new BufferSpan(stackalloc byte[10]);
        span.Position = 5;

        span.Seek(0, SeekOrigin.Begin);

        Assert.Equal(0, span.Position);
    }

    [Fact]
    public void Seek_End_UpdatesPosition()
    {
        var span = new BufferSpan(stackalloc byte[10]);
        span.Seek(-2, SeekOrigin.End);
        Assert.Equal(8, span.Position);
    }

    [Fact]
    public void Seek_CurrentNegativeOffsetWithinBounds_UpdatesPosition()
    {
        var span = new BufferSpan(stackalloc byte[10]);
        span.Position = 5;

        span.Seek(-3, SeekOrigin.Current);

        Assert.Equal(2, span.Position);
    }

    [Fact]
    public void Seek_Negative_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[10]);
            span.Seek(-1, SeekOrigin.Begin);
        });
    }

    [Fact]
    public void Seek_PastEnd_Throws()
    {
        Assert.Throws<EndOfBufferException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[10]);
            span.Seek(1, SeekOrigin.End);
        });
    }

    [Fact]
    public void Seek_InvalidOrigin_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var span = new BufferSpan(stackalloc byte[10]);
            span.Seek(0, (SeekOrigin)42);
        });
    }
}
