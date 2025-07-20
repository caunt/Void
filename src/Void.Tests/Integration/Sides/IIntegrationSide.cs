using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Void.Tests.Integration.Sides;

public interface IIntegrationSide : IAsyncDisposable
{
    public IEnumerable<string> Logs { get; }

    public Task RunAsync(CancellationToken cancellationToken);
    public Task SetupAsync(string workingDirectory, HttpClient client, CancellationToken cancellationToken = default);
}
