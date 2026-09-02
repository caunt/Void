using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Profiles;

/// <summary>
/// Represents the signed public key data used to identify Minecraft chat signatures.
/// </summary>
/// <param name="Revision">The key-signature layout and protocol applicability.</param>
/// <param name="ExpiresAt">The expiration timestamp encoded by the protocol.</param>
/// <param name="PublicKey">The DER-encoded subject public key. The supplied array is retained without copying.</param>
/// <param name="Signature">The Yggdrasil signature over the key data. The supplied array is retained without copying.</param>
public record IdentifiedKey(IdentifiedKeyRevision Revision, long ExpiresAt, byte[] PublicKey, byte[] Signature)
{
    /// <summary>
    /// The DER-encoded Yggdrasil session public key used to validate profile public-key signatures.
    /// </summary>
    public static readonly byte[] YggdrasilSessionPublicKey = Convert.FromBase64String("MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAylB4B6m5lz7jwrcFz6Fd/fnfUhcvlxsTSn5kIK/2aGG1C3kMy4VjhwlxF6BFUSnfxhNswPjh3ZitkBxEAFY25uzkJFRwHwVA9mdwjashXILtR6OqdLXXFVyUPIURLOSWqGNBtb08EN5fMnG8iFLgEJIBMxs9BvF3s3/FhuHyPKiVTZmXY0WY4ZyYqvoKR+XjaTRPPvBsDa4WI2u1zxXMeHlodT3lnCzVvyOYBLXL6CJgByuOxccJ8hnXfF9yY4F0aeL080Jz/3+EBNG8RO4ByhtBf4Ny8NQ6stWsjfeUIvH7bU/4zCYcYOq4WrInXHqS8qruDmIl7P5XXGcabuzQstPf/h2CRAUpP/PlHXcMlvewjmGU6MfDK+lifScNYwjPxRo4nKTGFZf/0aqHCh/EAsQyLKrOIYRE0lDG3bzBh8ogIMLAugsAfBb6M3mqCqKaTMAf/VAjh5FFJnjS+7bE+bZEV0qwax1CEoPPJL1fIQjOS8zj086gjpGRCtSy9+bTPTfTR/SJ+VUB5G2IeCItkNHpJX2ygojFZ9n5Fnj7R9ZnOM+L8nyIjPu3aePvtcrXlyLhH/hvOfIOjPxOlqW+O5QwSFP4OEcyLAUgDdUgyW36Z5mB285uKW/ighzZsOTevVUG2QwDItObIV6i8RCxFbN2oDHyPaO5j1tTaBNyVt8CAwEAAQ==");

    private bool? _isSignatureValid;

    /// <summary>
    /// Gets or sets the profile UUID linked to this key.
    /// </summary>
    /// <remarks>Assigning this property directly does not invalidate a previously cached <see cref="IsSignatureValid" /> value.</remarks>
    public Uuid ProfileUuid { get; set; }

    /// <summary>
    /// Gets or sets the cached result of validating the Yggdrasil key signature.
    /// </summary>
    /// <value>On first read, the signature is validated against <see cref="ProfileUuid" /> and the result is cached. An assigned value overrides that cached result.</value>
    public bool IsSignatureValid
    {
        get => _isSignatureValid ??= ValidateData(ProfileUuid);
        set => _isSignatureValid = value;
    }

    /// <summary>
    /// Verifies a SHA-256 PKCS#1 signature using this profile public key.
    /// </summary>
    /// <param name="signature">The signature bytes to verify.</param>
    /// <param name="data">The data bytes covered by the signature.</param>
    /// <returns><see langword="true" /> when verification succeeds; otherwise, <see langword="false" />. Invalid key material and cryptographic errors are reported as <see langword="false" />.</returns>
    public bool VerifyDataSignature(ReadOnlySpan<byte> signature, params ReadOnlySpan<byte> data)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(PublicKey, out _);

            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Associates a profile UUID with this key after validating its Yggdrasil signature.
    /// </summary>
    /// <param name="uuid">The profile UUID to associate.</param>
    /// <returns>
    /// <see langword="true" /> when the UUID is nonzero and either matches an already associated valid UUID or validates successfully; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>A successful first association sets <see cref="ProfileUuid" /> and caches <see cref="IsSignatureValid" /> as <see langword="true" />.</remarks>
    public bool AddUuid(Uuid uuid)
    {
        if (uuid == default)
            return false;

        if (ProfileUuid != default)
            return ProfileUuid.Equals(uuid) && IsSignatureValid;

        if (!ValidateData(uuid))
            return false;

        IsSignatureValid = true;
        ProfileUuid = uuid;

        return true;
    }

    private bool ValidateData(Uuid uuid)
    {
        Guid guid = uuid;

        if (Revision == IdentifiedKeyRevision.GenericV1Revision)
        {
            var publicKeyText = $"-----BEGIN RSA PUBLIC KEY-----\n{Convert.ToBase64String(PublicKey, Base64FormattingOptions.InsertLineBreaks)}\n-----END RSA PUBLIC KEY-----\n";
            var verifyText = ExpiresAt + publicKeyText.Replace("\r", string.Empty);
            Span<byte> verify = stackalloc byte[Encoding.ASCII.GetByteCount(verifyText)];
            Encoding.ASCII.GetBytes(verifyText, verify);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(YggdrasilSessionPublicKey, out _);

            return rsa.VerifyData(verify, Signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }
        else
        {
            if (guid == default)
                return false;

            Span<byte> verify = stackalloc byte[PublicKey.Length + 24];
            var buffer = new MinecraftBuffer(verify);
            buffer.WriteUuid(uuid);
            buffer.WriteLong(ExpiresAt);
            buffer.Write(PublicKey);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(YggdrasilSessionPublicKey, out _);

            return rsa.VerifyData(verify[..(int)buffer.Position], Signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }
    }
}

/// <summary>
/// Describes a key revision and the protocol versions it applies to.
/// </summary>
/// <param name="BackwardsCompatibleTo">Revision identifiers that remain valid for this revision.</param>
/// <param name="ApplicableTo">The protocol versions that use this revision.</param>
public record IdentifiedKeyRevision(IEnumerable<IdentifiedKeyRevision> BackwardsCompatibleTo, List<ProtocolVersion> ApplicableTo)
{
    /// <summary>
    /// The generic key revision introduced for Minecraft 1.19.
    /// </summary>
    public static readonly IdentifiedKeyRevision GenericV1Revision = new([], [ProtocolVersion.MINECRAFT_1_19]);

    /// <summary>
    /// The profile-linked key revision introduced for Minecraft 1.19.1.
    /// </summary>
    public static readonly IdentifiedKeyRevision LinkedV2Revision = new([], [ProtocolVersion.MINECRAFT_1_19_1]);
}
