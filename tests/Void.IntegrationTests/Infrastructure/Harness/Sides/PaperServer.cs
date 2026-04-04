using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Void.IntegrationTests.Infrastructure.Extensions;

namespace Void.IntegrationTests.Infrastructure.Harness.Sides;

public record PaperServer(IContainer Container) : IIntegrationSide
{
    private DateTime _readLogsSince = DateTime.UtcNow;

    public IEnumerable<string> Logs => ReadLogsAsync(_readLogsSince).GetAwaiter().GetResult();
    public int Port => Container.GetMappedPublicPort(containerPort: 25565);

    public static async Task<PaperServer> CreateAsync(CancellationToken cancellationToken = default)
    {
        var container = new ContainerBuilder("itzg/minecraft-server:latest")
            .WithPortBinding(port: 25565, assignRandomHostPort: true)
            .WithEnvironment("EULA", "TRUE")
            .WithEnvironment("TYPE", "PAPER")
            .WithEnvironment("VERSION", "1.21.4")
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
        var since = lookupHistory ? _readLogsSince : DateTime.Now;

        while (!cancellationToken.IsCancellationRequested)
        {
            var logs = await ReadLogsAsync(since, cancellationToken);

            if (logs.Any(line => line.Contains(text)))
                return;

            await Task.Delay(100, cancellationToken);
        }
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

    private async Task<IEnumerable<string>> ReadLogsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var (standardOutput, standardError) = await Container.GetLogsAsync(since, ct: cancellationToken);
        return Enumerate(standardError).Prepend("STDERR:").Append("STDOUT:").Concat(Enumerate(standardOutput));
        static IEnumerable<string> Enumerate(string text) => text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(line => line.Trim('\r'));
    }
}
