using System;
using System.Text;
using Void.Minecraft.Nbt;
using Void.Minecraft.Nbt.Tags;
using Xunit;

namespace Void.Tests.NbtTests;

public class NbtReaderTests
{
    [Theory]
    [InlineData(new byte[] { 0x0A, 0x00, 0x04, 0x72, 0x6F, 0x6F, 0x74, 0x08, 0x00, 0x00, 0x00, 0x05, 0x76, 0x61, 0x6C, 0x75, 0x65, 0x00 })]
    [InlineData(new byte[] { 0x0A, 0x00, 0x04, 0x72, 0x6F, 0x6F, 0x74, 0x08, 0x00, 0x01, 0x0A, 0x00, 0x05, 0x76, 0x61, 0x6C, 0x75, 0x65, 0x00 })]
    public void ReadCompound_ReplacesWhitespaceTagNameWithText(byte[] bytes)
    {
        NbtTag.Parse<NbtCompound>(bytes, out var result, readName: true);

        Assert.Equal("root", result.Name);
        Assert.True(result.ContainsKey("text"));
        var textTag = Assert.IsType<NbtString>(result["text"]);
        Assert.Equal("value", textTag.Value);
    }
}
