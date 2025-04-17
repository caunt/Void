using System;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Extensions;

public static class CommandServiceExtensions
{
    public static void Register(this ICommandService commands, Func<IArgumentContext, LiteralArgumentBuilder> configure)
    {
        commands.Dispatcher.Add(configure(default(ArgumentContext)).Build());
    }
}
