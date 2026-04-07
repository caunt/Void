using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PortableMinecraftClientImageFixture : IAsyncLifetime
{
    public const string DockerFileName = "PortableMinecraftClientDockerfile";

    private readonly string _temporaryContextDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    public IFutureDockerImage DockerImage { get => field ?? throw new InvalidOperationException($"{nameof(DockerImage)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(_temporaryContextDirectoryPath);

        File.Copy(
            Path.Combine(CommonDirectoryPath.GetProjectDirectory().DirectoryPath, DockerFileName),
            Path.Combine(_temporaryContextDirectoryPath, DockerFileName));

        DockerImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(_temporaryContextDirectoryPath)
            .WithDockerfile(DockerFileName)
            .WithContextDirectory(_temporaryContextDirectoryPath)
            .WithName($"{nameof(PortableMinecraftClient).ToLower()}:latest")
            .Build();

        await DockerImage.CreateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        // Allow caching image layers
        // await DockerImage.DisposeAsync();

        Directory.Delete(_temporaryContextDirectoryPath, recursive: true);

        GC.SuppressFinalize(this);
    }
}
