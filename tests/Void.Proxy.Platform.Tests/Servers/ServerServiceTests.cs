using Moq;
using NUnit.Framework;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;
using Void.Proxy.Servers; // For the concrete Server record
using System.Linq;

namespace Void.Proxy.Platform.Tests.Servers;

[TestFixture]
public class ServerServiceTests
{
    private Mock<ISettings> _mockSettings = null!;
    private Mock<ILinkService> _mockLinkService = null!;
    private ServerService _serverService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockSettings = new Mock<ISettings>();
        _mockLinkService = new Mock<ILinkService>();

        // Default setups
        _mockSettings.Setup(s => s.Servers).Returns(new List<IServer>());
        _mockLinkService.Setup(l => l.All).Returns(new List<ILink>());

        _serverService = new ServerService(_mockSettings.Object, _mockLinkService.Object);
    }

    private int GetAddedServersCount()
    {
        var settingsServers = _mockSettings.Object.Servers ?? Enumerable.Empty<IServer>();
        var linkServers = _mockLinkService.Object.All?.Select(l => l.Server) ?? Enumerable.Empty<IServer>();
        return _serverService.All.Count(s => !settingsServers.Contains(s) && !linkServers.Contains(s));
    }

    [Test]
    public async Task GetOrAddServerAsync_ServerExistsInSettings_ReturnsFromServerSettings()
    {
        // Arrange
        var expectedServer = new Server("TestServer", "host1.com", 25565);
        _mockSettings.Setup(s => s.Servers).Returns(new List<IServer> { expectedServer });

        // Act
        var actualServer = await _serverService.GetOrAddServerAsync(expectedServer.Host, expectedServer.Port);

        // Assert
        Assert.That(actualServer, Is.SameAs(expectedServer));
        Assert.That(GetAddedServersCount(), Is.EqualTo(0), "No server should have been added to the dynamic list.");
    }

    [Test]
    public async Task GetOrAddServerAsync_ServerExistsInActiveLinks_ReturnsFromServerLinks()
    {
        // Arrange
        var expectedServer = new Server("LinkServer", "host2.com", 25566);
        var mockLink = new Mock<ILink>();
        mockLink.Setup(l => l.Server).Returns(expectedServer);
        _mockLinkService.Setup(l => l.All).Returns(new List<ILink> { mockLink.Object });

        // Act
        var actualServer = await _serverService.GetOrAddServerAsync(expectedServer.Host, expectedServer.Port);

        // Assert
        Assert.That(actualServer, Is.SameAs(expectedServer));
        Assert.That(GetAddedServersCount(), Is.EqualTo(0), "No server should have been added to the dynamic list.");
    }

    [Test]
    public async Task GetOrAddServerAsync_ServerExistsInDynamicallyAddedList_ReturnsExistingInstance()
    {
        // Arrange
        var host = "dynamic.host.com";
        var port = 25567;

        // Ensure not in settings or links
        _mockSettings.Setup(s => s.Servers).Returns(new List<IServer>());
        _mockLinkService.Setup(l => l.All).Returns(new List<ILink>());

        // Act
        var firstCallServer = await _serverService.GetOrAddServerAsync(host, port);
        var addedCountAfterFirstCall = GetAddedServersCount();
        var secondCallServer = await _serverService.GetOrAddServerAsync(host, port);
        var addedCountAfterSecondCall = GetAddedServersCount();

        // Assert
        Assert.That(secondCallServer, Is.SameAs(firstCallServer));
        Assert.That(addedCountAfterFirstCall, Is.EqualTo(1), "Server should have been added on the first call.");
        Assert.That(addedCountAfterSecondCall, Is.EqualTo(1), "No new server should have been added on the second call.");
    }

    [Test]
    public async Task GetOrAddServerAsync_ServerDoesNotExist_CreatesAndAddsNewServer()
    {
        // Arrange
        var host = "new.server.com";
        var port = 25568;
        var expectedName = $"{host}:{port}";

        // Ensure not in settings or links
        _mockSettings.Setup(s => s.Servers).Returns(new List<IServer>());
        _mockLinkService.Setup(l => l.All).Returns(new List<ILink>());

        // Act
        var newServer = await _serverService.GetOrAddServerAsync(host, port);

        // Assert
        Assert.That(newServer, Is.Not.Null);
        Assert.That(newServer.Host, Is.EqualTo(host));
        Assert.That(newServer.Port, Is.EqualTo(port));
        Assert.That(newServer.Name, Is.EqualTo(expectedName));
        
        Assert.That(GetAddedServersCount(), Is.EqualTo(1), "One server should have been added to the dynamic list.");
        
        var allServers = _serverService.All.ToList();
        Assert.That(allServers, Does.Contain(newServer), "The 'All' property should include the new server.");
        Assert.That(allServers.Count(s => s.Host == host && s.Port == port), Is.EqualTo(1), "New server should be unique in All list based on host/port.");
    }
}
