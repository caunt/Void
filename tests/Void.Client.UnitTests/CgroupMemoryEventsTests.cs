using Void.Client;
using Xunit;

namespace Void.Client.UnitTests;

public sealed class CgroupMemoryEventsTests
{
    [Fact]
    public void ParsesOutOfMemoryKillCount()
    {
        const string content = "low 0\nhigh 0\noom 3\noom_kill 2\noom_group_kill 0\n";

        Assert.Equal(2, CgroupMemoryEvents.ParseOutOfMemoryKillCount(content));
    }

    [Theory]
    [InlineData("")]
    [InlineData("oom 1")]
    [InlineData("oom_kill invalid")]
    public void MissingOrInvalidOutOfMemoryKillCountReturnsNull(string content)
    {
        Assert.Null(CgroupMemoryEvents.ParseOutOfMemoryKillCount(content));
    }
}
