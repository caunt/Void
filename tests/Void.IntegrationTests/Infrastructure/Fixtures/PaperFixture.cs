using System;
using System.Threading;
using System.Threading.Tasks;
using Void.IntegrationTests.Infrastructure.Harness;
using Void.IntegrationTests.Infrastructure.Harness.Sides;
using Xunit;

namespace Void.IntegrationTests.Infrastructure.Fixtures;

public class PaperFixture : IAsyncLifetime
{
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public PaperServer PaperServer1 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer1)} is not initialized."); set; }
    public PaperServer PaperServer2 { get => field ?? throw new InvalidOperationException($"{nameof(PaperServer2)} is not initialized."); set; }

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

            var paperServer1 = PaperServer.CreateAsync(Timeouts.SetupTimeoutToken);
            var paperServer2 = PaperServer.CreateAsync(Timeouts.SetupTimeoutToken);

            PaperServer1 = await paperServer1;
            PaperServer2 = await paperServer2;

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

        await PaperServer1.DisposeAsync();
        await PaperServer2.DisposeAsync();
    }
}
