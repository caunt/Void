using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using Serilog;
using Void.Proxy.Configuration;
using Void.Proxy.Models.General;
using Void.Proxy.Network.Protocol.Registry;
using Void.Proxy.Utils;

namespace Void.Proxy;

public static class Proxy
{
    public static readonly Settings Settings;
    public static readonly ILogger Logger;
    public static readonly JsonSerializerOptions JsonSerializerOptions;
    public static readonly RSACryptoServiceProvider RSA;
    public static readonly HttpClient HttpClient;
    public static readonly List<Link> Links;
    public static readonly Dictionary<string, ServerInfo> Servers;

    static Proxy()
    {
        RSA = new RSACryptoServiceProvider();
        HttpClient = new HttpClient();
        Links = [];
        Servers = [];

        Settings = Settings.LoadAsync().GetAwaiter().GetResult();
        Settings.Servers.ForEach(RegisterServer);

        Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Is(Settings.LogLevel).CreateLogger();

        JsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonIPAddressConverter());
        JsonSerializerOptions.Converters.Add(new JsonIPEndPointConverter());

        Registries.Fill();
    }

    public static async Task StartAsync()
    {
        var listener = new TcpListener(Settings.Address, Settings.Port);
        listener.Start();

        Logger.Information($"Listening for connections on port {Settings.Port}...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            var serverInfo = Servers.Values.ElementAt(0);
            var server = serverInfo.CreateTcpClient();

            var link = new Link(client, server);
            link.SetServerInfo(serverInfo);

            Links.Add(link);
        }
    }

    public static void RegisterServer(ServerInfo serverInfo)
    {
        Servers.Add(serverInfo.Name, serverInfo);
    }
}