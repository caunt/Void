using MinecraftProxy.Models.General;
using MinecraftProxy.Network.Protocol.Forwarding;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;

namespace MinecraftProxy;

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

    public static readonly HttpClient HttpClient = new();
    public static readonly RSACryptoServiceProvider RSA = new(1024);
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    public static readonly LoggerConfiguration LoggerConfiguration = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Is(LogLevel);
    public static readonly ILogger Logger = LoggerConfiguration.CreateLogger();
    public static readonly List<Link> Links = new();

    public static async Task StartAsync()
    {
        var listener = new TcpListener(IPAddress.Any, ListenPort);
        listener.Start();

        Logger.Information($"Listening for connections on port {ListenPort}...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            var serverInfo = Servers.Values.First();
            var server = serverInfo.CreateTcpClient();

            var link = new Link(client, server);
            link.SetServerInfo(serverInfo);

            Links.Add(link);
        }
    }
}