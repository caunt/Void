using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Network.Protocol.States;

namespace Void.Proxy.API.Network.IO.Messages;

public interface IMinecraftPacket : IMinecraftMessage
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}

public delegate TSelf DecodeDelegate<out TSelf>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TSelf : IMinecraftPacket;

public interface IMinecraftPacket<in TState, out TSelf> : IMinecraftPacket where TState : IProtocolState where TSelf : IMinecraftPacket
{
    public static abstract TSelf Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
    Task<bool> HandleAsync(TState state);
}