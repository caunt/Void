using System.Security.Cryptography;

namespace Void.Proxy.Api.Crypto;

public interface ICryptoService
{
    public RSACryptoServiceProvider Instance { get; }
}
