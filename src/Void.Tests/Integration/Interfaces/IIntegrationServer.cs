namespace Void.Tests.Integration.Interfaces;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

internal interface IIntegrationServer
{
    Task<Process> StartAsync(ConnectionTestBase test, CancellationToken cancellationToken);
}
