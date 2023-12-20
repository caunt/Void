using IniParser;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using Void.Proxy.Configuration;
using Void.Proxy.Models.General;
using Void.Proxy.Network.Protocol.Forwarding;

namespace Void.Proxy;

public static class Proxy
{
    public static readonly LogEventLevel LogLevel = LogEventLevel.Debug;
    public static readonly int ListenPort = 25565;
    public static readonly int CompressionThreshold = 256;
    public static readonly Dictionary<string, ServerInfo> Servers = new()
    {
        { "server1", new ServerInfo("127.0.0.1", 25566, new NoneForwarding()) },
        { "server2", new ServerInfo("127.0.0.1", 25567, new NoneForwarding()) },
        { "server3", new ServerInfo("127.0.0.1", 25568, new NoneForwarding()) }
    };

    public static readonly ILogger Logger;
    public static readonly JsonSerializerOptions JsonSerializerOptions;
    public static readonly Settings Settings;
    public static readonly RSACryptoServiceProvider RSA;
    public static readonly List<Link> Links;
    public static readonly HttpClient HttpClient;

    static Proxy()
    {
        Logger = new LoggerConfiguration().WriteTo.Console(LogLevel).CreateLogger();
        JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        Settings = Settings.LoadAsync().GetAwaiter().GetResult();

        RSA = new();
        Links = [];
        HttpClient = new();
    }

    public static async Task StartAsync()
    {
        var listener = new TcpListener(IPAddress.Any, ListenPort);
        listener.Start();

        Logger.Information($"Listening for connections on port {ListenPort}...");

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
}