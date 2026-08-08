using System.Security.Cryptography;

namespace Void.Proxy.Api.Crypto;

/// <summary>
/// Provides the process-wide RSA provider used by proxy authentication and encryption services.
/// </summary>
public interface ICryptoService
{
    /// <summary>
    /// Gets the shared RSA cryptographic provider.
    /// </summary>
    /// <value>The provider that owns the proxy RSA key pair.</value>
    public RSACryptoServiceProvider Instance { get; }
}
