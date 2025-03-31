using Void.Minecraft.Commands;
using Void.Minecraft.Commands.Brigadier;
using Void.Proxy.Api.Commands;

namespace Void.Proxy.Commands;

public class CommandService : ICommandService
{
    private readonly CommandDispatcher _dispatcher = new();

    public async ValueTask ExecuteAsync(ICommandSource source, string command)
    {
        var result = await _dispatcher.Execute(command, source);
    }
}
