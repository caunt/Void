﻿using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier;

public delegate ValueTask<int> CommandExecutor(CommandContext context);
