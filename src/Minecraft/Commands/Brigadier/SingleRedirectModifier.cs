using Void.Minecraft.Commands.Brigadier.Context;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Maps a redirect context to one command source.</summary>
/// <param name="context">The redirect context.</param>
/// <returns>The source used by the next command stage.</returns>
public delegate ICommandSource SingleRedirectModifier(CommandContext context);
