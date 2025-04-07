using System.IO;

namespace Void.Minecraft.Network.Messages.Binary;

public interface IMinecraftBinaryMessage : IMinecraftMessage
{
    public int Id { get; }
    public MemoryStream Stream { get; }
}
