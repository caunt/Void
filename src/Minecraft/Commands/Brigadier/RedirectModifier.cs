using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Maps a redirect context to zero or more command sources.</summary>
/// <param name="source">The redirect context.</param>
/// <returns>The sources used by the next command stage.</returns>
public delegate IEnumerable<ICommandSource> RedirectModifier(CommandContext source);
