using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using Void.Proxy.Configuration;
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
    public static readonly List<Link> Links;
    public static readonly Dictionary<string, ServerInfo> Servers;

    static Proxy()
    {
        RSA = new();
        HttpClient = new();
        Links = [];
        Servers = [];

        Settings = Settings.LoadAsync().GetAwaiter().GetResult();
        Settings.Servers.ForEach(RegisterServer);

        Logger = new LoggerConfiguration().WriteTo.Console(Settings.LogLevel).CreateLogger();

        JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonIPAddressConverter());
        JsonSerializerOptions.Converters.Add(new JsonIPEndPointConverter());
    }

    public static async Task StartAsync()
    {
        var listener = new TcpListener(IPAddress.Any, Settings.Port);
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

    public static void RegisterServer(ServerInfo serverInfo) => Servers.Add(serverInfo.Name, serverInfo);
}