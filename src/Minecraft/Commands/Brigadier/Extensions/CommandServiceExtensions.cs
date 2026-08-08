using System;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Extensions;

/// <summary>Provides Brigadier builder registration for the proxy command service.</summary>
public static class CommandServiceExtensions
{
    /// <summary>Builds a root literal command and adds it to the service dispatcher.</summary>
    /// <param name="commands">The command service.</param>
    /// <param name="configure">The builder factory, invoked with the default argument context.</param>
    public static void Register(this ICommandService commands, Func<IArgumentContext, LiteralArgumentBuilder> configure)
    {
        commands.Dispatcher.Add(configure(default(ArgumentContext)).Build());
    }
}
