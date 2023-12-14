using Void.Proxy.Models.General;

namespace Void.Proxy.Commands;

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

        // can't be awaited because we need to release both channels before switch will happen
        _ = link.SwitchServerAsync(serverInfo).ContinueWith(task =>
        {
            if (!task.IsCompletedSuccessfully)
                Proxy.Logger.Error($"Player {link.Player} redirection caused exception:\n{task.Exception}");
        });

        return true;
    }
}
