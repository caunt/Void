using Serilog;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using Void.Proxy.Configuration;
using Void.Proxy.Events;
using Void.Proxy.Models.General;
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
    public static readonly List<Link> Links;
    public static readonly Dictionary<string, ServerInfo> Servers;

    private static readonly LoggerConfiguration _loggerConfiguration;

    static Proxy()
    {
        RSA = new();
        HttpClient = new();
        Plugins = new();
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
        await Plugins.LoadAsync(cancellationToken: cancellationToken);

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

        var serverInfo = Servers.Values.ElementAt(0);
        link.Connect(serverInfo);

        await link.ServerChannel!.WriteMessageAsync(handshake);
        link.StartForwarding();

        Links.Add(link);
    }

    public static void RegisterServer(ServerInfo serverInfo) => Servers.Add(serverInfo.Name, serverInfo);
}