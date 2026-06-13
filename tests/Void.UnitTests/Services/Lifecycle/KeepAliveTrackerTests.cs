using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_12_1_to_v1_12_2;
using Void.Proxy.Plugins.Common.Network.Registries.Transformations.Mappings;
using Void.Proxy.Plugins.Common.Services.Lifecycle;
using Xunit;

namespace Void.UnitTests.Services.Lifecycle;

public class KeepAliveTrackerTests
{
    [Fact]
    public void UsesLegacyRequestIdWidth_ReturnsExpectedBoundary()
    {
        Assert.True(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.MINECRAFT_1_7_2));
        Assert.True(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.MINECRAFT_1_12_1));
        Assert.False(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.MINECRAFT_1_12_2));
        Assert.False(KeepAliveTracker.UsesLegacyRequestIdWidth(ProtocolVersion.Latest));
    }

    [Fact]
    public void CreateRequestId_ForLegacyVersion_ReturnsSignedIntRange()
    {
        for (var i = 0; i < 256; i++)
        {
            var id = KeepAliveTracker.CreateRequestId(ProtocolVersion.MINECRAFT_1_12_1);

            Assert.InRange(id, int.MinValue, int.MaxValue);
        }
    }

    [Fact]
    public void LegacyWidthRequestIds_SurviveKeepAliveTransformationRoundTrip()
    {
        var ids = new long[] { int.MinValue, -1, 0, 1962467597, int.MaxValue };

        foreach (var id in ids)
            Assert.Equal(id, TransformLongThroughLegacyWireFormat(id));
    }

    [Fact]
    public void ArbitraryLongRequestId_DoesNotSurviveKeepAliveTransformationRoundTrip()
    {
        const long sentId = 1983115045386117389;
        const long receivedId = 1962467597;

        var transformedId = TransformLongThroughLegacyWireFormat(sentId);

        Assert.Equal(receivedId, transformedId);
        Assert.NotEqual(sentId, transformedId);
    }

    private static long TransformLongThroughLegacyWireFormat(long id)
    {
        var transformation = new KeepAliveTransformation1_12_2();

        using var longStream = new MemoryStream();
        var longBuffer = new MinecraftBuffer(longStream);
        longBuffer.WriteLong(id);
        longStream.Position = 0;

        using var varIntStream = Transform(longStream, transformation.Downgrade);
        using var transformedStream = Transform(varIntStream, transformation.Upgrade);
        var transformedBuffer = new MinecraftBuffer(transformedStream);

        return transformedBuffer.ReadLong();
    }

    private static MemoryStream Transform(MemoryStream inputStream, MinecraftPacketTransformation transformation)
    {
        var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(0, inputStream));
        transformation(wrapper);
        wrapper.Reset();

        var outputStream = new MemoryStream();
        var outputBuffer = new MinecraftBuffer(outputStream);
        wrapper.WriteProcessedValues(ref outputBuffer);
        outputStream.Position = 0;

        return outputStream;
    }
}
