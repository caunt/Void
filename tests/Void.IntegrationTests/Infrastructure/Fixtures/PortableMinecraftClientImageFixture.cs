using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PortableMinecraftClientImageFixture : IAsyncLifetime
{
    public const string DockerFileName = "PortableMinecraftClientDockerfile";

    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private readonly string _temporaryContextDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private bool _initialized;

    public IFutureDockerImage DockerImage { get => field ?? throw new InvalidOperationException($"{nameof(DockerImage)} is not initialized."); set; }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask EnsureInitializedAsync()
    {
        if (_initialized)
            return;

        await _initializationLock.WaitAsync();

        try
        {
            if (_initialized)
                return;

            Directory.CreateDirectory(_temporaryContextDirectoryPath);

            var projectDirectoryPath = CommonDirectoryPath.GetProjectDirectory().DirectoryPath;

            foreach (var fileName in new[] { DockerFileName, "start-display", "send-chat" })
            {
                File.Copy(
                    Path.Combine(projectDirectoryPath, fileName),
                    Path.Combine(_temporaryContextDirectoryPath, fileName));
            }

            DockerImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(_temporaryContextDirectoryPath)
                .WithDockerfile(DockerFileName)
                .WithContextDirectory(_temporaryContextDirectoryPath)
                .WithCreateParameterModifier(parameters => parameters.Platform = "linux/amd64")
                .WithName($"{nameof(PortableMinecraftClient).ToLower()}:latest")
                .WithCleanUp(cleanUp: false)
                .Build();

            await DockerImage.CreateAsync();

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _initializationLock.Dispose();

        if (!_initialized)
            return;

        await DockerImage.DisposeAsync();

        Directory.Delete(_temporaryContextDirectoryPath, recursive: true);
    }
}
