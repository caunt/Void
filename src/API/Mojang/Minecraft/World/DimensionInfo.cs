namespace Void.Proxy.API.Mojang.Minecraft.World;

public record DimensionInfo(string RegistryIdentifier, string? LevelName, bool IsFlat, bool IsDebugType);