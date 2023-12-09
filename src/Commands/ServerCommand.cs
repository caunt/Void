using MinecraftProxy.Models.General;

namespace MinecraftProxy.Commands;

public static class ServerCommand
{
    public static async Task<bool> ExecuteAsync(Link link, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            await link.Player.SendMessageAsync($"Available servers: {string.Join(", ", Proxy.Servers.Keys)}\nUsage: /server <name>");
            return true;
        }

        var serverName = arguments[0];

        if (!Proxy.Servers.TryGetValue(serverName, out var serverInfo))
        {
            await link.Player.SendMessageAsync($"Server {serverName} not found");
            return true;
        }

        await link.Player.SendMessageAsync($"Switch server to {serverName} ({serverInfo.Host}:{serverInfo.Port})");
        await link.SwitchServerAsync(serverInfo);
        return true;
    }
}
