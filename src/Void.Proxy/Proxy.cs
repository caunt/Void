using Serilog;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using Void.Proxy.API;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.Configuration;
using Void.Proxy.Events;
using Void.Proxy.Network.IO;
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

    public static int MaxHandshakeSize { get; private set; } = 4096;

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

        Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}")
            .MinimumLevel.Is(Settings.LogLevel)
            .CreateLogger()
            .ForContext("SourceContext", nameof(Proxy));

        JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonIPAddressConverter());
        JsonSerializerOptions.Converters.Add(new JsonIPEndPointConverter());

    }

    public static async Task ExecuteAsync(CancellationToken cancellationToken)
    {
#if DEBUG
        var directory = new DirectoryInfo(Environment.CurrentDirectory);

        while (directory != null && directory.Name != "src")
            directory = directory.Parent;

        var pluginsDirectory = Path.Combine(Environment.CurrentDirectory, "plugins");

        foreach (var pluginDirectory in directory!.GetDirectories("Void.Proxy.Plugins.*"))
        {
            if (pluginDirectory.Name.EndsWith("API"))
                continue;

            foreach (var file in Directory.GetFiles(Path.Combine(pluginDirectory.FullName, "bin", "Debug", "net8.0"), "*.dll"))
                File.Copy(file, Path.Combine(pluginsDirectory, Path.GetFileName(file)), true);
        }
#endif

        await Plugins.LoadAsync(cancellationToken: cancellationToken);
        await Events.ThrowAsync<ProxyStart>();

        var listener = new TcpListener(Settings.Address, Settings.Port);
        listener.Start();

        Logger.Information($"Listening for connections on port {Settings.Port}...");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync(cancellationToken);

                _ = ProcessClientAsync(client, cancellationToken).ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                        return;

                    Logger.Error($"Client {client.Client?.RemoteEndPoint?.ToString() ?? "Disposed?"} cannot be proxies:\n{task.Exception}");
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // cancellation token set
            }
        }

        // global cancellation token already canceled

        Logger.Information("Stopping proxy");
        await Events.ThrowAsync<ProxyStop>();
        await Plugins.UnloadAsync();
    }

    private static async Task ProcessClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        var buffer = new byte[MaxHandshakeSize];
        var length = await client.GetStream().ReadAsync(buffer, cancellationToken);

        var searchClientProtocolCodec = new SearchClientProtocolCodec { Buffer = buffer[..length] };
        await Events.ThrowAsync(searchClientProtocolCodec, cancellationToken);

        var clientChannel = searchClientProtocolCodec.Result switch
        {
            { Channel: { } _channel } => _channel,
            _ => new SimpleMinecraftChannel(client.GetStream())
        };

        if (clientChannel is SimpleMinecraftChannel simpleMinecraftChannel)
        {
            if (searchClientProtocolCodec.Result is null)
                simpleMinecraftChannel.Inject(searchClientProtocolCodec.Buffer);
            else
                simpleMinecraftChannel.Inject(searchClientProtocolCodec.Result.NextBuffer);
        }

        var link = new Link(client, clientChannel);
        var serverInfo = Servers.Values.ElementAt(0);

        var searchServerProtocolCodec = new SearchServerProtocolCodec { Link = link, Server = serverInfo };
        await Events.ThrowAsync(searchServerProtocolCodec, cancellationToken);

        var serverChannel = searchServerProtocolCodec.Result switch
        {
            { } _channel => _channel,
            _ => new SimpleMinecraftChannel(serverInfo.CreateTcpClient().GetStream())
        };

        link.Connect(serverInfo, serverChannel);
        link.StartForwarding();

        Links.Add(link);
    }

    public static void RegisterServer(ServerInfo serverInfo) => Servers.Add(serverInfo.Name, serverInfo);
}