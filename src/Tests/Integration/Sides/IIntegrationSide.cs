using System;
using System.Collections.Generic;

namespace Void.Tests.Integration.Sides;

public interface IIntegrationSide : IAsyncDisposable
{
    public IEnumerable<string> Logs { get; }
}
