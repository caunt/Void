using System.Security.Cryptography;
using Void.Proxy.API.Crypto;

namespace Void.Proxy.Crypto;

public class RsaCryptoService : ICryptoService
{
    public readonly RSACryptoServiceProvider Instance = new();
}