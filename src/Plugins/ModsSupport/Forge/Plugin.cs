using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Plugins;
using Void.Proxy.Plugins.ModsSupport.Forge.Services;

namespace Void.Proxy.Plugins.ModsSupport.Forge;

public class Plugin(IDependencyService dependencies) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range();

    public string Name => nameof(Forge);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        dependencies.Register(services =>
        {
            services.AddScoped<HandshakeService>();
        });
    }
}


// (HandshakePacket packet)
// var addressParts = packet.ServerAddress.Split('\0', StringSplitOptions.RemoveEmptyEntries);
// var isForge = ForgeMarker.Range().Any(marker => addressParts.Contains(marker.Value));
// 
// if (isForge)
//     link.Player.SetClientType(ClientType.Forge);
// else if (addressParts.Length > 1)
//     Console.WriteLine($"Player {link.Player} had extra marker(s) {string.Join(", ", addressParts[1..])} in handshake, ignoring");
// 
// link.SetProtocolVersion(ProtocolVersion.Get(packet.ProtocolVersion));
// link.SwitchState(packet.NextState);
// link.SaveHandshake(packet);

// public class ForgeMarker
// {
//     private static readonly List<ForgeMarker> _markers = [];
// 
//     public static readonly ForgeMarker Instance = new("FORGE");
// 
//     public static ForgeMarker? Longest => _markers.MaxBy(marker => marker.Value.Length);
// 
//     public string Value { get; init; }
// 
//     public ForgeMarker(string value)
//     {
//         Value = value;
//         _markers.Add(this);
//     }
// 
//     public static IEnumerable<ForgeMarker> Range() => _markers.AsReadOnly();
// }
