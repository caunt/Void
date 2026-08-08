using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Observes a command execution attempt.</summary>
/// <param name="context">The execution context.</param>
/// <param name="success">Whether execution succeeded.</param>
/// <param name="result">The command result, or zero for a reported failure.</param>
public delegate void ResultConsumer(CommandContext context, bool success, int result);
