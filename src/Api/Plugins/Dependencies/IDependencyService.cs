﻿
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;

namespace Void.Proxy.Api.Plugins.Dependencies;

public interface IDependencyService : IEventListener
{
    // public IServiceProvider Services { get; }

    public TService CreateInstance<TService>();
    public TService CreateInstance<TService>(Type serviceType);
    public object CreateInstance(Type serviceType);
    public TService? GetService<TService>();
    public TService GetRequiredService<TService>() where TService : notnull;
    public TService Get<TService>(Func<IServiceProvider, TService> configure);
    public void Register(Action<ServiceCollection> configure, bool activate = true);
}
