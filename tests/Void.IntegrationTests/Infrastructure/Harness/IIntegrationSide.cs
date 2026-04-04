using System;
using System.Collections.Generic;

namespace Void.IntegrationTests.Infrastructure.Harness;

public interface IIntegrationSide : IAsyncDisposable
{
    public IEnumerable<string> Logs { get; }

    public void ClearLogs();
}
