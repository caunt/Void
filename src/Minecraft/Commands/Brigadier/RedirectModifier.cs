﻿using System.Collections.Generic;
using Void.Common.Commands;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier;

public delegate IEnumerable<ICommandSource> RedirectModifier(CommandContext source);
