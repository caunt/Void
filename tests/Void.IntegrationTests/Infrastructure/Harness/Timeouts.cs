using System;
using System.Threading;

namespace Void.IntegrationTests.Infrastructure.Harness;

public static class Timeouts
{
    public static TimeSpan SetupTimeout { get; } = TimeSpan.FromMinutes(10);
    public static TimeSpan StepTimeout { get; } = TimeSpan.FromSeconds(90);
    public static CancellationToken SetupTimeoutToken => new CancellationTokenSource(SetupTimeout).Token;
    public static CancellationToken StepTimeoutToken => new CancellationTokenSource(StepTimeout).Token;
}
