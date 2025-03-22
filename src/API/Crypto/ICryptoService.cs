using System.Security.Cryptography;

namespace Void.Proxy.API.Crypto;

public interface ICryptoService
{
    public RSACryptoServiceProvider Instance { get; }
}