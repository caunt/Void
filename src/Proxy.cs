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

    public static readonly HttpClient HttpClient = new();
    public static readonly RSACryptoServiceProvider RSA = new(1024);
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    public static readonly LoggerConfiguration LoggerConfiguration = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Is(LogLevel);
    public static readonly ILogger Logger = LoggerConfiguration.CreateLogger();

    public static async Task StartAsync()
    {
        var listener = new TcpListener(IPAddress.Any, ListenPort);
        listener.Start();

        Logger.Information($"Listening for connections on port {ListenPort}...");

        while (true)
        {
            _ = await listener.AcceptTcpClientAsync()
                .ContinueWith(async task =>
                {
                    Logger.Information($"Client connection accepted from {task.Result.Client.RemoteEndPoint}");
                    using var player = new Player(task.Result);
                    using var server = new Server("127.0.0.1", 25566, new NoneForwarding() /*new ModernForwarding("aaa")*/).Init();
                    await player.ForwardTrafficAsync(server);
                }).CatchForwardingExceptions();
        }
    }

    public static async Task<bool> ExecuteCommandAsync(Player player, Server server, string command)
    {
        async Task<bool> HandleServerCommandAsync(string[] arguments)
        {
            await player.SendMessageAsync($"Switch server to {arguments.FirstOrDefault() ?? "not specified"}");
            return true;
        }

        var parts = command.Split(' ');

        return parts[0] switch
        {
            "server" => await HandleServerCommandAsync(parts[1..]),
            _ => false
        };
    }

    private static Task<Task> CatchForwardingExceptions(this Task<Task> task)
    {
        return task.ContinueWith(async task =>
        {
            var forwardingTask = await task;

            try
            {
                await forwardingTask;
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Unhandled forwarding exception: {exception}");
            }
        });
    }
}