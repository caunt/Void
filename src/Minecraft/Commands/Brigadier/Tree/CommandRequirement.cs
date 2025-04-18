using System.Threading;
using System.Threading.Tasks;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Tree;

public delegate ValueTask<bool> CommandRequirement(ICommandSource source, CancellationToken cancellationToken);
