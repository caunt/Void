using Void.Proxy.Api.Network.Streams;

namespace Void.Minecraft.Network.Streams;

/// <summary>
/// Identifies a layered message stream used by the Minecraft protocol pipeline.
/// </summary>
public interface IMinecraftStream : IMessageStream;
