using Void.Proxy.Models.General;

namespace Void.Proxy.Commands;

public static class CommandExecutor
{
    public static async Task<bool> ExecuteAsync(Link link, string command)
    {
        var parts = command.Split(' ');

        return parts[0] switch
        {
            "server" => await ServerCommand.ExecuteAsync(link, parts[1..]),
            _ => false
        };
    }
}
