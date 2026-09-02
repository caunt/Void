using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class GameRuntimeMemoryTests
{
    [Theory]
    [InlineData(2048, "--jvm-arg=-Xmx2048M")]
    [InlineData(4096, "--jvm-arg=-Xmx4096M")]
    public void CreatesMaximumHeapArgument(int memoryMb, string expected)
    {
        Assert.Equal(expected, GameRuntime.CreateMaximumHeapArgument(memoryMb));
    }
}
