namespace Void.Proxy.Plugins.ModsSupport.Forge;

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