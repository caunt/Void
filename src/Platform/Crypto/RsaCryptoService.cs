using System.Security.Cryptography;
using Void.Proxy.Api.Crypto;

namespace Void.Proxy.Crypto;

public class RsaCryptoService : ICryptoService
{
    public RSACryptoServiceProvider Instance { get; } = new();
}