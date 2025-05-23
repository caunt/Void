using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Services;
using Void.Proxy.Api.Settings;
using Void.Proxy.Links;
using Microsoft.Extensions.Hosting;

namespace Void.Proxy.Platform.Tests.Links;

[TestFixture]
public class LinkServiceTests
{
    private Mock<ILogger<LinkService>> _mockLogger = null!;
    private Mock<ISettings> _mockSettings = null!;
    private Mock<IEventService> _mockEventService = null!;
    private Mock<IServerService> _mockServerService = null!;
    private Mock<IHostApplicationLifetime> _mockHostApplicationLifetime = null!;

    // Use a partial mock for LinkService to spy on the virtual ConnectAsync method
    private Mock<LinkService> _mockLinkService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<LinkService>>();
        _mockSettings = new Mock<ISettings>();
        _mockEventService = new Mock<IEventService>();
        _mockServerService = new Mock<IServerService>();
        _mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();

        // Setup LinkService with mocked dependencies
        // We create a partial mock of LinkService to be able to verify calls to its own virtual methods
        _mockLinkService = new Mock<LinkService>(
            _mockLogger.Object,
            _mockSettings.Object,
            _mockEventService.Object,
            _mockServerService.Object,
            _mockHostApplicationLifetime.Object
        ) { CallBase = true }; // Important: CallBase ensures non-mocked methods behave normally
    }

    [Test]
    public async Task ConnectAsync_WithHostAndPort_ServerExists_CallsOverloadWithExistingServer()
    {
        // Arrange
        var mockPlayer = new Mock<IPlayer>();
        var mockExistingServer = new Mock<IServer>();
        var expectedResult = ConnectionResult.Connected; // Example result
        var host = "test.server.com";
        var port = 25565;

        _mockServerService
            .Setup(s => s.GetOrAddServerAsync(host, port, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockExistingServer.Object);

        // Setup the virtual ConnectAsync(IPlayer, IServer, CancellationToken) to be verifiable
        _mockLinkService
            .Setup(ls => ls.ConnectAsync(mockPlayer.Object, mockExistingServer.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var actualResult = await _mockLinkService.Object.ConnectAsync(mockPlayer.Object, host, port, CancellationToken.None);

        // Assert
        _mockServerService.Verify(s => s.GetOrAddServerAsync(host, port, It.IsAny<CancellationToken>()), Times.Once);
        _mockLinkService.Verify(ls => ls.ConnectAsync(mockPlayer.Object, mockExistingServer.Object, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task ConnectAsync_WithHostAndPort_NewServer_CallsOverloadWithNewServer()
    {
        // Arrange
        var mockPlayer = new Mock<IPlayer>();
        var mockNewServer = new Mock<IServer>(); // Represents a newly created/added server
        var expectedResult = ConnectionResult.GracefulDisconnect; // Example result, can be different
        var host = "another.server.com";
        var port = 12345;

        _mockServerService
            .Setup(s => s.GetOrAddServerAsync(host, port, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockNewServer.Object);
        
        _mockLinkService
            .Setup(ls => ls.ConnectAsync(mockPlayer.Object, mockNewServer.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var actualResult = await _mockLinkService.Object.ConnectAsync(mockPlayer.Object, host, port, CancellationToken.None);

        // Assert
        _mockServerService.Verify(s => s.GetOrAddServerAsync(host, port, It.IsAny<CancellationToken>()), Times.Once);
        _mockLinkService.Verify(ls => ls.ConnectAsync(mockPlayer.Object, mockNewServer.Object, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }
}
