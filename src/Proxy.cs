﻿using MinecraftProxy.Network.Protocol.Forwarding;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;

namespace MinecraftProxy;

public static class Proxy
{
    public static readonly LogEventLevel LogLevel = LogEventLevel.Verbose;
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
            var player = new Player(await listener.AcceptTcpClientAsync());
            var server = new Server("127.0.0.1", 25566, new NoneForwarding() /*new ModernForwarding("aaa")*/);
            _ = player.ForwardTrafficAsync(server);
        }
    }
}