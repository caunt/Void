using Void.IntegrationTests.Infrastructure.Fixtures;
using Xunit;

#if DEBUG
[assembly: CaptureConsole]
#endif

[assembly: CollectionBehavior(MaxParallelThreads = 0)] // 0 means Environment.ProcessorCount
[assembly: AssemblyFixture(typeof(PaperFixture))]
[assembly: AssemblyFixture(typeof(PortableMinecraftClientImageFixture))]
