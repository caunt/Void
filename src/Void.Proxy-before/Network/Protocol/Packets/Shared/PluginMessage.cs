using System.Text;
using System.Text.RegularExpressions;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Custom;

namespace Void.Proxy.Network.Protocol.Packets.Shared;

public partial struct PluginMessage : IMinecraftPacket<IConfigurePlayState>
{
    public const string BRAND_CHANNEL_LEGACY = "MC|Brand";
    public const string BRAND_CHANNEL = "minecraft:brand";
    public const string REGISTER_CHANNEL_LEGACY = "REGISTER";
    public const string REGISTER_CHANNEL = "minecraft:register";
    public const string UNREGISTER_CHANNEL_LEGACY = "UNREGISTER";
    public const string UNREGISTER_CHANNEL = "minecraft:unregister";

    public string Identifier { get; set; }
    public byte[] Data { get; set; }
    public int MaxDataSize { get; }
    public Direction Direction { get; }

    public PluginMessage() : this(0, 0) { throw new NotSupportedException("Specify direction and size limit to plugin message"); }

    public PluginMessage(Direction direction, int maxDataSize) => (Direction, MaxDataSize) = (direction, maxDataSize);

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_13)
            buffer.WriteString(TransformLegacyToModernChannel(Identifier));
        else
            buffer.WriteString(Identifier);

        if (protocolVersion < ProtocolVersion.MINECRAFT_1_8)
            buffer.WriteVarShort(Data.Length);

        if (Data.Length > MaxDataSize)
            throw new InternalBufferOverflowException($"Plugin message max size is {MaxSize} bytes");

        buffer.Write(Data);
    }

    public async Task<bool> HandleAsync(IConfigurePlayState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Identifier = buffer.ReadString();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_13)
            Identifier = TransformLegacyToModernChannel(Identifier);
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_8)
            Data = buffer.ReadToEnd().ToArray();
        else
            Data = buffer.Read(buffer.ReadVarShort()).ToArray();

        if (Data.Length > MaxDataSize)
            throw new InternalBufferOverflowException($"Plugin message max size is {MaxSize} bytes");
    }

    public int MaxSize() => 0
        + Encoding.UTF8.GetByteCount(Identifier) + 5
        + Data.Length + 5;

    private string TransformLegacyToModernChannel(string name)
    {
        if (name.Contains(':'))
            return name;

        return name switch
        {
            REGISTER_CHANNEL_LEGACY => REGISTER_CHANNEL,
            UNREGISTER_CHANNEL_LEGACY => UNREGISTER_CHANNEL,
            BRAND_CHANNEL_LEGACY => BRAND_CHANNEL,
            _ => "legacy:" + INVALID_IDENTIFIER_REGEX().Replace(name.ToLower(), string.Empty)
        };
    }

    [GeneratedRegex("[^a-z0-9\\-_]*")]
    private static partial Regex INVALID_IDENTIFIER_REGEX();
}

