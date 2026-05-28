using Void.IntegrationTests.Infrastructure.Fixtures;
using Xunit;

#if DEBUG
[assembly: CaptureConsole]
#endif

[assembly: CollectionBehavior(MaxParallelThreads = 3)]
[assembly: AssemblyFixture(typeof(PaperFixture))]
[assembly: AssemblyFixture(typeof(PortableMinecraftClientImageFixture))]
