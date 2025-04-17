namespace Void.Proxy.Hosting;

public class InterceptingServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    public IServiceCollection CreateBuilder(IServiceCollection services) => services;

    public IServiceProvider CreateServiceProvider(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        return new InterceptingServiceProvider(provider);
    }
}
