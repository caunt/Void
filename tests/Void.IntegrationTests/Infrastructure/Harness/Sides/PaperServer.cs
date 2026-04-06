using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
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
            .WithPortBinding(port: 25565, assignRandomHostPort: true)
            .WithEnvironment("EULA", "TRUE")
            .WithEnvironment("TYPE", "PAPER")
            .WithEnvironment("VERSION", "1.21.4")
            .WithEnvironment("DIFFICULTY", "peaceful")
            .WithEnvironment("ONLINE_MODE", "FALSE")
            .WithEnvironment("MODRINTH_PROJECTS", "viaversion,viabackwards,viarewind")
            .WithEnvironment("JVM_OPTS", "-Dpaper.playerconnection.keepalive=120")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("For help, type \"help\"", options => options.WithTimeout(TimeSpan.FromMinutes(5))))
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
