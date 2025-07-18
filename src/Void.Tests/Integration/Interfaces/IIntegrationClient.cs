namespace Void.Tests.Integration.Interfaces;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

internal interface IIntegrationClient
{
    Task<Process> StartAsync(ConnectionTestBase test, CancellationToken cancellationToken);
}
