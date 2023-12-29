namespace Void.Proxy.Network.IO.Common;

/*public class PacketStream(Stream baseStream) : IPacketStream
{
    public async Task FlushAsync(CancellationToken cancellationToken) => await baseStream.FlushAsync(cancellationToken);

    public async ValueTask<MinecraftMessage> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        var length = await baseStream.ReadVarIntAsync(cancellationToken);
        return await baseStream.ReadMessageAsync(length, cancellationToken);
    }

    public async ValueTask WritePacketAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        await baseStream.WriteVarIntAsync(message.Length + MinecraftBuffer.GetVarIntSize(message.PacketId), cancellationToken);
        await baseStream.WriteVarIntAsync(message.PacketId, cancellationToken);
        await baseStream.WriteAsync(message.Memory, cancellationToken);
    }
}*/