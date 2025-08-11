using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins;
using Void.Proxy.Plugins.Context;
using Void.Proxy.Plugins.Containers;
using Void.Proxy.Plugins.Dependencies;
using Xunit;

namespace Void.Tests.Plugins;

public class PluginServiceTests
{
    [Fact]
    public async Task LoadEnvironmentPluginsAsync_InvalidUrl_DoesNotThrow()
    {
        var previous = Environment.GetEnvironmentVariable("VOID_PLUGINS");
        Environment.SetEnvironmentVariable("VOID_PLUGINS", "https://example.org/download/RateLimiter.dll");

        try
        {
            var command = new RootCommand();
            PluginService.RegisterOptions(command);
            var parseResult = command.Parse([]);
            var context = new InvocationContext(parseResult);

            var service = new PluginService(
                NullLogger<PluginService>.Instance,
                new DummyEventService(),
                new DummyDependencyService(),
                context,
                new HttpClient(new StubHttpMessageHandler()));

            await service.LoadEnvironmentPluginsAsync();

            Assert.Empty(service.All);
        }
        finally
        {
            Environment.SetEnvironmentVariable("VOID_PLUGINS", previous);
        }
    }

    [Fact]
    public async Task LoadEnvironmentPluginsAsync_InvalidPath_DoesNotThrow()
    {
        var path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, "not an assembly");

        var previous = Environment.GetEnvironmentVariable("VOID_PLUGINS");
        Environment.SetEnvironmentVariable("VOID_PLUGINS", path);

        try
        {
            var command = new RootCommand();
            PluginService.RegisterOptions(command);
            var parseResult = command.Parse([]);
            var context = new InvocationContext(parseResult);

            var service = new PluginService(
                NullLogger<PluginService>.Instance,
                new DummyEventService(),
                new DummyDependencyService(),
                context,
                new HttpClient(new StubHttpMessageHandler()));

            await service.LoadEnvironmentPluginsAsync();

            Assert.Empty(service.All);
        }
        finally
        {
            Environment.SetEnvironmentVariable("VOID_PLUGINS", previous);
        }
    }

    [Fact]
    public async Task LoadEnvironmentPluginsAsync_FileUrl_LoadsPlugin()
    {
        var baseDir = AppContext.BaseDirectory;
        var repoRoot = Path.GetFullPath(Path.Combine(baseDir, "../../../../../"));
        var pluginDir = Path.Combine(repoRoot, "src", "Plugins", "ExamplePlugin", "bin");
        var pluginPath = Directory.GetFiles(pluginDir, "Void.Proxy.Plugins.ExamplePlugin.dll", SearchOption.AllDirectories).First();

        var previous = Environment.GetEnvironmentVariable("VOID_PLUGINS");
        Environment.SetEnvironmentVariable("VOID_PLUGINS", new Uri(pluginPath).AbsoluteUri);

        try
        {
            var command = new RootCommand();
            PluginService.RegisterOptions(command);
            var parseResult = command.Parse([]);
            var context = new InvocationContext(parseResult);

            var service = new PluginService(
                NullLogger<PluginService>.Instance,
                new DummyEventService(),
                new SimpleDependencyService(),
                context,
                new HttpClient(new StubHttpMessageHandler()));

            await service.LoadEnvironmentPluginsAsync();

            Assert.Single(service.All);
            Assert.Equal("ExamplePlugin", service.All.First().Name);
        }
        finally
        {
            Environment.SetEnvironmentVariable("VOID_PLUGINS", previous);
        }
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }

    private sealed class DummyEventService : IEventService
    {
        public IEnumerable<IEventListener> Listeners => Enumerable.Empty<IEventListener>();

        public ValueTask ThrowAsync<T>(CancellationToken cancellationToken = default) where T : IEvent, new() => ValueTask.CompletedTask;
        public ValueTask ThrowAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent => ValueTask.CompletedTask;
        public ValueTask<TResult?> ThrowWithResultAsync<TResult>(IEventWithResult<TResult> @event, CancellationToken cancellationToken = default) => ValueTask.FromResult<TResult?>(default);
        public ValueTask<TResult?> ThrowWithResultAsync<T, TResult>(CancellationToken cancellationToken = default) where T : IEventWithResult<TResult?>, new() => ValueTask.FromResult<TResult?>(default);
        public void RegisterListeners(IEnumerable<IEventListener> listeners, CancellationToken cancellationToken = default) { }
        public void RegisterListeners(CancellationToken cancellationToken = default, params IEventListener[] listeners) { }
        public void RegisterListeners(params IEventListener[] listeners) { }
        public void UnregisterListeners(IEnumerable<IEventListener> listeners) { }
        public void UnregisterListeners(params IEventListener[] listeners) { }
    }

    private sealed class DummyDependencyService : IDependencyService
    {
        public IEnumerable<IEventListener> Listeners => Enumerable.Empty<IEventListener>();

        public TService CreateInstance<TService>(CancellationToken cancellationToken = default, params object[] parameters) => throw new BadImageFormatException();
        public TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters) => throw new BadImageFormatException();
        public object CreateInstance(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters) => throw new BadImageFormatException();
        public bool TryGetScopedPlayerContext(object instance, out IPlayerContext context) { context = default!; return false; }
        public IServiceProvider CreatePlayerComposite(IPlayer player) => new ServiceCollection().BuildServiceProvider();
        public void ActivatePlayerContext(IPlayerContext context) { }
        public void DisposePlayerContext(IPlayerContext context) { }
        public TService? GetService<TService>() => default;
        public object? GetService(Type serviceType) => null;
        public void Register(Action<ServiceCollection> configure, bool activate = true) { }
    }

    private sealed class SimpleDependencyService : IDependencyService
    {
        public IEnumerable<IEventListener> Listeners => Enumerable.Empty<IEventListener>();

        public TService CreateInstance<TService>(CancellationToken cancellationToken = default, params object[] parameters)
        {
            return (TService)CreateInstance(typeof(TService), cancellationToken, parameters);
        }

        public TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters)
        {
            return (TService)CreateInstance(serviceType, cancellationToken, parameters);
        }

        public object CreateInstance(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters)
        {
            if (serviceType == typeof(PluginAssemblyLoadContext))
            {
                return new PluginAssemblyLoadContext(
                    NullLogger<PluginAssemblyLoadContext>.Instance,
                    this,
                    (string)parameters[0],
                    (Stream)parameters[1],
                    (IReadOnlyCollection<WeakPluginContainer>)parameters[2]);
            }

            if (serviceType == typeof(DependencyResolver))
            {
                return new DependencyResolver(
                    NullLogger<DependencyResolver>.Instance,
                    new StubNuGetDependencyResolver(),
                    new StubEmbeddedDependencyResolver(),
                    new StubFileDependencyResolver(),
                    (AssemblyLoadContext)parameters[0],
                    (IReadOnlyCollection<WeakPluginContainer>)parameters[1]);
            }

            if (typeof(IPlugin).IsAssignableFrom(serviceType))
            {
                return Activator.CreateInstance(serviceType, NullLogger.Instance, this)!;
            }

            return Activator.CreateInstance(serviceType, parameters)!;
        }

        public bool TryGetScopedPlayerContext(object instance, out IPlayerContext context)
        {
            context = default!;
            return false;
        }

        public IServiceProvider CreatePlayerComposite(IPlayer player) => new ServiceCollection().BuildServiceProvider();

        public void ActivatePlayerContext(IPlayerContext context) { }

        public void DisposePlayerContext(IPlayerContext context) { }

        public TService? GetService<TService>() => default;

        public object? GetService(Type serviceType) => null;

        public void Register(Action<ServiceCollection> configure, bool activate = true) { }

        private sealed class StubNuGetDependencyResolver : INuGetDependencyResolver
        {
            public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName) => null;
            public void AddRepository(string uri) { }
        }

        private sealed class StubEmbeddedDependencyResolver : IEmbeddedDependencyResolver
        {
            public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName) => null;
        }

        private sealed class StubFileDependencyResolver : IFileDependencyResolver
        {
            public Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName) => null;
            public string? ResolveUnmanagedDllToPath(string unmanagedDllName) => null;
        }
    }
}

