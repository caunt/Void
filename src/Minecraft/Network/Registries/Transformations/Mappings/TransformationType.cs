namespace Void.Minecraft.Network.Registries.Transformations.Mappings;

/// <summary>
/// Identifies whether packet fields require no transformation, an upgrade, or a downgrade.
/// </summary>
public enum TransformationType
{
    /// <summary>No transformation is required.</summary>
    None,
    /// <summary>Fields are transformed toward a newer protocol layout.</summary>
    Upgrade,
    /// <summary>Fields are transformed toward an older protocol layout.</summary>
    Downgrade
}
