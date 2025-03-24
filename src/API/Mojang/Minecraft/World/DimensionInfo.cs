namespace Void.Proxy.Api.Mojang.Minecraft.World;

public record DimensionInfo(string RegistryIdentifier, string? LevelName, bool IsFlat, bool IsDebugType);