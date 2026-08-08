namespace Void.Minecraft.World;

/// <summary>
/// Describes dimension fields carried by Java Edition join-game and respawn packets.
/// </summary>
/// <param name="RegistryIdentifier">The namespaced identifier of the dimension or dimension type, according to the packet version.</param>
/// <param name="LevelName">The optional world or level identifier used by protocol versions that transmit it.</param>
/// <param name="IsFlat">Whether the dimension uses flat world generation.</param>
/// <param name="IsDebugType">Whether the dimension is a debug world.</param>
public record DimensionInfo(
    string RegistryIdentifier,
    string? LevelName,
    bool IsFlat,
    bool IsDebugType);
