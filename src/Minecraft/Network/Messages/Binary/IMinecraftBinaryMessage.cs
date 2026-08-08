using System.IO;

namespace Void.Minecraft.Network.Messages.Binary;

/// <summary>
/// Represents a Minecraft packet retained in binary form, including its numeric identifier.
/// </summary>
public interface IMinecraftBinaryMessage : IMinecraftMessage
{
    /// <summary>Gets the decoded numeric packet identifier.</summary>
    public int Id { get; }
    /// <summary>
    /// Gets the stream containing the binary packet data.
    /// </summary>
    /// <remarks>The message implementation owns this stream and is expected to dispose it when the message is disposed.</remarks>
    public MemoryStream Stream { get; }
}
