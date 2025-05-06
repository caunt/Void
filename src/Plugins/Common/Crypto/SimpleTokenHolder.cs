namespace Void.Proxy.Plugins.Common.Crypto;

public class SimpleTokenHolder : ITokenHolder
{
    private readonly Dictionary<TokenType, ReadOnlyMemory<byte>> _tokens = [];

    public ReadOnlyMemory<byte> Get(TokenType type)
    {
        return _tokens[type];
    }

    public bool Has(TokenType type)
    {
        return _tokens.ContainsKey(type);
    }

    public void Store(TokenType type, ReadOnlyMemory<byte> token)
    {
        _tokens[type] = token;
    }

    public bool Remove(TokenType type)
    {
        return _tokens.Remove(type);
    }
}
