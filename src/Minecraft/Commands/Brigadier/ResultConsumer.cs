using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier;

public delegate void ResultConsumer(CommandContext context, bool success, int result);
