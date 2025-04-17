using Void.Proxy.Api.Commands;

namespace Void.Proxy.Commands.Extensions;

public static class CommandServiceExtensions
{
    public static void RegisterDefault(this ICommandService commands)
    {
        // commands.Register(builder => builder.Literal("stop").Executes(StopServer));
        // commands.Register(builder => builder.Literal("kick").Then(builder.Argument("username", Arguments.String()).Suggests(SuggestPlayer).Executes(KickPlayerAsync)));
    }
}
