using Void.IntegrationTests.Infrastructure.Fixtures;
using Xunit;

#if DEBUG
[assembly: CaptureConsole]
#endif

[assembly: AssemblyFixture(typeof(PortableMinecraftClientImageFixture))]
