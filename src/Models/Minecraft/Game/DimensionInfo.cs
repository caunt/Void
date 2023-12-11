namespace MinecraftProxy.Models.Minecraft.Game;

public class DimensionInfo(string registryIdentifier, string levelName, bool isFlat, bool isDebugType)
{
    public string RegistryIdentifier { get; set; } = registryIdentifier;
    public string LevelName { get; set; } = levelName ?? string.Empty;
    public bool IsFlat { get; set; } = isFlat;
    public bool IsDebugType { get; set; } = isDebugType;
}