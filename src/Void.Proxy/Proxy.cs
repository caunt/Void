using Serilog;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.Configuration;
using Void.Proxy.Events;
using Void.Proxy.Models.General;
using Void.Proxy.Plugins;
using Void.Proxy.Utils;

namespace Void.Proxy;

public static class Proxy
{
    public static readonly Settings Settings;
    public static readonly ILogger Logger;
    public static readonly JsonSerializerOptions JsonSerializerOptions;
    public static readonly RSACryptoServiceProvider RSA;
    public static readonly HttpClient HttpClient;
    public static readonly PluginManager Plugins;
    public static readonly EventManager Events;
    public static readonly List<Link> Links;
    public static readonly Dictionary<string, ServerInfo> Servers;

    private static readonly LoggerConfiguration _loggerConfiguration;

    static Proxy()
    {
        RSA = new();
        HttpClient = new();
        Plugins = new();
        Events = new();
        Links = [];
        Servers = [];

        Settings = Settings.LoadAsync().GetAwaiter().GetResult();
        Settings.Servers.ForEach(RegisterServer);

        _loggerConfiguration = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Is(Settings.LogLevel);
        Logger = _loggerConfiguration.CreateLogger();

        JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonIPAddressConverter());
        JsonSerializerOptions.Converters.Add(new JsonIPEndPointConverter());

    }

    public static async Task ExecuteAsync(CancellationToken cancellationToken)
    {
#if DEBUG
        var directory = new DirectoryInfo(Environment.CurrentDirectory);

        while (directory != null && directory.GetDirectories("Void.Proxy.ExamplePlugin").Length == 0)
            directory = directory.Parent;

        await Plugins.LoadAsync(Path.Combine(directory!.FullName, "Void.Proxy.ExamplePlugin", "bin", "Debug", "net8.0"), cancellationToken: cancellationToken);
#else
        await Plugins.LoadAsync(cancellationToken: cancellationToken);
#endif

        var listener = new TcpListener(Settings.Address, Settings.Port);
        listener.Start();

        Logger.Information($"Listening for connections on port {Settings.Port}...");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync(cancellationToken);

                _ = ProcessClientAsync(client).ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                        return;

                    Logger.Error($"Client {client.Client?.RemoteEndPoint?.ToString() ?? "Disposed?"} cannot be proxies:\n{task.Exception}");
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Logger.Information("Stopping proxy");
            }
        }

        // global cancellation token already canceled
        await Plugins.UnloadAsync();
    }

    private static async Task ProcessClientAsync(TcpClient client)
    {
        var link = new Link(client);

        var handshake = await link.PlayerChannel.ReadMessageAsync();
        await Events.ThrowAsync(new SearchProtocolCodec());

        var serverInfo = Servers.Values.ElementAt(0);
        link.Connect(serverInfo);

        await link.ServerChannel!.WriteMessageAsync(handshake);
        link.StartForwarding();

        Links.Add(link);
    }

    public static void RegisterServer(ServerInfo serverInfo) => Servers.Add(serverInfo.Name, serverInfo);
}