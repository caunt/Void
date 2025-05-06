namespace Void.Proxy.Plugins.Common.Crypto;

public interface ITokenHolder
{
    public ReadOnlyMemory<byte> Get(TokenType type);
    public bool Has(TokenType type);
    public void Store(TokenType type, ReadOnlyMemory<byte> token);
    public bool Remove(TokenType type);
}
