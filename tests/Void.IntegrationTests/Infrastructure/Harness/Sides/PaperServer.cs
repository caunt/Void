using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Void.IntegrationTests.Infrastructure.Extensions;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PaperServer(IContainer Container) : IIntegrationSide
{
    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<string> Logs => Container.ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();
    public int Port => Container.GetMappedPublicPort(containerPort: 25565);

    public static async Task<PaperServer> CreateAsync(CancellationToken cancellationToken = default)
    {
        var container = new ContainerBuilder("itzg/minecraft-server:latest")
            .WithImagePullPolicy(PullPolicy.Always)
            .WithPortBinding(port: 25565, assignRandomHostPort: true)
            .WithEnvironment("EULA", "TRUE")
            .WithEnvironment("TYPE", "PAPER")
            .WithEnvironment("VERSION", "1.21.4")
            .WithEnvironment("DIFFICULTY", "peaceful")
            .WithEnvironment("ONLINE_MODE", "FALSE")
            .WithEnvironment("MODRINTH_PROJECTS", "viaversion,viabackwards,viarewind")
            .WithEnvironment("JVM_OPTS", "-Dpaper.playerconnection.keepalive=120")
            .WithEnvironment("PATCH_DEFINITIONS", "/patches")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("For help, type \"help\"", options => options.WithTimeout(TimeSpan.FromMinutes(5))))
            .WithResourceMapping(
                Encoding.UTF8.GetBytes("""
                {
                  "file": "/data/bukkit.yml",
                  "ops": [
                    {
                      "$set": {
                        "path": "$.settings['connection-throttle']",
                        "value": -1,
                        "value-type": "int"
                      }
                    }
                  ]
                }
                """),
                FilePath.Of("/patches/connection-throttle.json"))
            .Build();

        await container.StartAsync(cancellationToken);

        return new PaperServer(container);
    }

    public async Task ExpectTextAsync(string text, bool lookupHistory = false, CancellationToken cancellationToken = default)
    {
        if (lookupHistory)
            await Container.ExpectTextAsync(text, since: _readLogsSince, cancellationToken);
        else
            await Container.ExpectTextAsync(text, cancellationToken);
    }

    public void ClearLogs()
    {
        _readLogsSince = DateTime.UtcNow;
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
