using System.Text.Json.Serialization;
using Void.Proxy.Utils;

namespace Void.Proxy.Models.Minecraft.Profile;

public class GameProfile
{
    [JsonConverter(typeof(GuidConverter))]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Property> Properties { get; set; }
}