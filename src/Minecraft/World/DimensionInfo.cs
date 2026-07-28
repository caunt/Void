namespace Void.Minecraft.World;

public record DimensionInfo(
    string RegistryIdentifier,
    string? LevelName,
    /// <summary>
    /// Gets a value indicating whether the dimension uses flat world generation.
    /// </summary>
    bool IsFlat,
    bool IsDebugType);
