namespace Void.Minecraft.Network.Registries.Transformations.Mappings;

/// <summary>
/// Transforms packet fields through a binary packet wrapper.
/// </summary>
/// <param name="wrapper">The stateful wrapper used to read, replace, omit, or append packet properties.</param>
public delegate void MinecraftPacketTransformation(IMinecraftBinaryPacketWrapper wrapper);
