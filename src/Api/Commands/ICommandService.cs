using Void.Minecraft.Commands;

namespace Void.Proxy.Api.Commands;

public interface ICommandService
{
    public ValueTask ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default);
}
