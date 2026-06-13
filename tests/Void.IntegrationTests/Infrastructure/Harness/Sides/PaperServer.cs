using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Void.IntegrationTests.Infrastructure.Extensions;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PaperServer(IContainer Container, string LogFileName) : IIntegrationSide
{
    private DateTime _readLogsSince = DateTime.UtcNow;
    public int Port => Container.GetMappedPublicPort(containerPort: 25565);

    public IEnumerable<string> Logs => ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();

    public void ClearLogs()
    {
        _readLogsSince = DateTime.UtcNow;
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public static async Task<PaperServer> CreateAsync(CancellationToken cancellationToken = default)
    {
        return await CreateAsync("server.log", cancellationToken);
    }

    public static async Task<PaperServer> CreateAsync(string logFileName, CancellationToken cancellationToken = default)
    {
        var container = new ContainerBuilder("itzg/minecraft-server:latest")
            .WithImagePullPolicy(PullPolicy.Always)
            .WithPortBinding(port: 25565, assignRandomHostPort: true)
            .WithEnvironment("EULA", "TRUE")
            .WithEnvironment("TYPE", "PAPER")
            .WithEnvironment("VERSION", "1.21.4")
            .WithEnvironment("DIFFICULTY", "peaceful")
            .WithEnvironment("MODE", "creative")
            .WithEnvironment("VIEW_DISTANCE", "3")
            .WithEnvironment("SIMULATION_DISTANCE", "3")
            .WithEnvironment("ONLINE_MODE", "FALSE")
            .WithEnvironment("MODRINTH_PROJECTS", "viaversion,viabackwards,viarewind")
            .WithEnvironment("JVM_OPTS", "-Dpaper.playerconnection.keepalive=120")
            .WithEnvironment("PATCH_DEFINITIONS", "/patches")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("For help, type \"help\"", options => options.WithTimeout(TimeSpan.FromMinutes(5))))
            .WithResourceMapping([
                    ..
                    """
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
                    """u8
                ],
                FilePath.Of("/patches/connection-throttle.json"))
            .Build();

        await container.StartAsync(cancellationToken);

        return new PaperServer(container, logFileName);
    }

    public async Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await Container.ReadLogsAsync(since, cancellationToken);
    }

    public async Task ExpectTextAsync(string text, bool lookupHistory = false, CancellationToken cancellationToken = default)
    {
        if (lookupHistory)
            await Container.ExpectTextAsync(text, since: _readLogsSince, cancellationToken);
        else
            await Container.ExpectTextAsync(text, cancellationToken);
    }
}
