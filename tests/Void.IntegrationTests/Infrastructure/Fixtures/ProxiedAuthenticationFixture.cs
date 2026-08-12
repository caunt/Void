using System;
using System.IO;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class ProxiedAuthenticationFixture : IAsyncLifetime
{
    public PaperServer PaperServer { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer)} is not initialized."); set; }
    public VoidProxy VoidProxy { get => field ?? throw new InvalidOperationException($"{nameof(VoidProxy)} is not initialized."); set; }

    public async ValueTask InitializeAsync()
    {
        PaperServer = await PaperServer.CreateAsync("server-full.log", Timeouts.SetupTimeoutToken, maximumPlayers: 0);

        try
        {
            VoidProxy = await VoidProxy.CreateAsync(Path.Combine(Path.GetTempPath(), nameof(ProxiedAuthenticationFixture), Path.GetRandomFileName()), $"localhost:{PaperServer.Port}", cancellationToken: Timeouts.SetupTimeoutToken);
        }
        catch
        {
            await PaperServer.DisposeAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await VoidProxy.LogWriter.WriteLineAsync($"Stopping {nameof(VoidProxy)} because of {nameof(ProxiedAuthenticationFixture)} disposal");
        await VoidProxy.DisposeAsync();
        await PaperServer.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
