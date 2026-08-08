using System.Threading;
using System.Threading.Tasks;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Tree;

/// <summary>Determines asynchronously whether a command source may use a node.</summary>
/// <param name="source">The command source.</param>
/// <param name="cancellationToken">A token that may cancel the check.</param>
/// <returns>Whether access is permitted.</returns>
public delegate ValueTask<bool> CommandRequirement(ICommandSource source, CancellationToken cancellationToken);
