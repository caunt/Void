using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier;

public delegate IEnumerable<ICommandSource> RedirectModifier(CommandContext source);
