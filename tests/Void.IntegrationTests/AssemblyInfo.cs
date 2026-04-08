using Void.IntegrationTests.Infrastructure.Fixtures;
using Xunit;

#if DEBUG
[assembly: CaptureConsole]
#endif

[assembly: CollectionBehavior(MaxParallelThreads = 10)]
[assembly: AssemblyFixture(typeof(PortableMinecraftClientImageFixture))]
