using System.Threading.Tasks;

namespace Void.Tests.Integration.Sides.Servers;

public interface IIntegrationServer : IIntegrationSide
{
    public Task ServerLoadingTask { get; }
}
