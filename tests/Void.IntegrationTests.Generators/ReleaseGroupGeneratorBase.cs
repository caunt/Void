using System.Text;
using Microsoft.CodeAnalysis;

namespace Void.IntegrationTests.Generators;

public abstract class ReleaseGroupGeneratorBase : IIncrementalGenerator
{
    public abstract void Initialize(IncrementalGeneratorInitializationContext context);

    protected static void AppendTestMethodForReleases(StringBuilder sourceBuilder, IEnumerable<string> releases)
    {
        sourceBuilder.AppendLine("    public static global::Xunit.TheoryData<global::Void.Minecraft.Network.ProtocolVersion> ProtocolVersions { get; } =");
        sourceBuilder.AppendLine("    [");

        foreach (var release in releases)
            sourceBuilder.AppendLine($"        global::Void.Minecraft.Network.ProtocolVersion.MINECRAFT_{release},");

        sourceBuilder.AppendLine("    ];");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("    [global::Xunit.Theory]");
        sourceBuilder.AppendLine("    [global::Xunit.MemberData(nameof(ProtocolVersions))]");
        sourceBuilder.AppendLine("    public async global::System.Threading.Tasks.Task PortableMinecraftClientConnectsToPaperServer_WithProtocolVersion(global::Void.Minecraft.Network.ProtocolVersion protocolVersion)");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        await base.RunAsync(protocolVersion);");
        sourceBuilder.AppendLine("    }");
    }
}
