using Void.IntegrationTests.Infrastructure.Fixtures;
using Xunit;

#if DEBUG
[assembly: CaptureConsole]
#endif

#if GITHUB_ACTIONS
[assembly: CollectionBehavior(MaxParallelThreads = 1)]
#else
[assembly: CollectionBehavior(MaxParallelThreads = 3)]
#endif

[assembly: AssemblyFixture(typeof(PaperFixture))]
