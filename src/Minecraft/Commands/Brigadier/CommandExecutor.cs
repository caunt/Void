using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>
/// Represents a synchronous command callback that computes an integer result from a <see cref="CommandContext"/>.
/// </summary>
/// <param name="context">The command execution context.</param>
/// <returns>The integer result returned to the command dispatcher.</returns>
public delegate int CommandExecutorSync(CommandContext context);

public delegate ValueTask<int> CommandExecutor(CommandContext context, CancellationToken cancellationToken);
