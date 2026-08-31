namespace Void.Proxy.Plugins.Common.Services.Entities;

public class EntityIdState
{
    private readonly Lock _lock = new();
    private int? _clientEntityId;
    private int? _serverEntityId;

    public void Update(int serverEntityId)
    {
        using var _ = _lock.EnterScope();

        _clientEntityId ??= serverEntityId;
        _serverEntityId = serverEntityId;
    }

    public bool TryGetIds(out int clientEntityId, out int serverEntityId)
    {
        using var _ = _lock.EnterScope();

        if (_clientEntityId is not { } client || _serverEntityId is not { } server)
        {
            clientEntityId = default;
            serverEntityId = default;
            return false;
        }

        clientEntityId = client;
        serverEntityId = server;
        return true;
    }
}
