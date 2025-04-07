namespace Void.Proxy.Plugins.Common.Crypto;

public class SimpleTokenHolder : ITokenHolder
{
    private readonly Dictionary<TokenType, ReadOnlyMemory<byte>> _tokens = [];

    public ReadOnlySpan<byte> Get(TokenType type)
    {
        return _tokens[type].Span;
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
