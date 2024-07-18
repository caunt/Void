using System.Text;
using Void.Proxy.Models.General;
using Void.Proxy.Network.Protocol.Packets.Clientbound;
using Void.Proxy.Network.Protocol.Packets.Shared;
using Void.Proxy.Network.Protocol.Registry;
using Void.Proxy.Network.Protocol.States.Custom;

namespace Void.Proxy.Network.Protocol.States.Common;

public class ConfigurationState(Link link) : ProtocolState, ILoginConfigurePlayState, IConfigurePlayState
{
    protected override StateRegistry Registry { get; } = Registries.ConfigurationStateRegistry;

    public Task<bool> HandleAsync(PluginMessage packet)
    {
        if (packet.Identifier == "minecraft:brand")
        {
            if (packet.Direction == Direction.Serverbound)
                link.Player.SetBrand(Encoding.UTF8.GetString(packet.Data[1..]));
            else if (packet.Direction == Direction.Clientbound)
                link.Server.SetBrand(Encoding.UTF8.GetString(packet.Data[1..]));

            return Task.FromResult(false);
        }

        if (packet.Identifier == "forge:handshake")
        {
            Proxy.Logger.Debug($"Received {packet.Direction} Configuration {packet.Identifier} with {packet.Data.Length} bytes");
            return Task.FromResult(false);
        }

        if (packet.Identifier == "minecraft:register")
        {
            var channels = Encoding.UTF8.GetString(packet.Data)
                .Split('\0', StringSplitOptions.RemoveEmptyEntries);
            Proxy.Logger.Debug($"Received {packet.Direction} Configuration register channels message: {string.Join(", ", channels)}");

            return Task.FromResult(false);
        }

        if (packet.Identifier == "minecraft:unregister")
        {
            var channels = Encoding.UTF8.GetString(packet.Data)
                .Split('\0', StringSplitOptions.RemoveEmptyEntries);
            Proxy.Logger.Debug($"Received {packet.Direction} Configuration unregister channels message: {string.Join(", ", channels)}");

            return Task.FromResult(false);
        }

        if (new[] { "minecraft", "forge", "fml" }.Any(name => packet.Identifier.Contains(name, StringComparison.InvariantCultureIgnoreCase)))
            Proxy.Logger.Debug($"Received {packet.Direction} Configuration plugin message in channel {packet.Identifier} with {packet.Data.Length} bytes");
        else
            Proxy.Logger.Verbose($"Received {packet.Direction} Configuration plugin message in channel {packet.Identifier} with {packet.Data.Length} bytes");

        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(FinishConfiguration packet)
    {
        link.SwitchState(4);
        return Task.FromResult(false);
    }
}