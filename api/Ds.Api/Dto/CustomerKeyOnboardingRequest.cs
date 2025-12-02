using Ds.Core.Entities;

namespace Ds.Api.Dto;

/// <summary>
/// Request to onboard or rotate a customer signing key
/// </summary>
public record CustomerKeyOnboardingRequest
{
    /// <summary>
    /// Public key base64 representation
    /// </summary>
    /// <example>MFkwEwYHKoZI...</example>
    public required string PublicKey { get; set; }

    /// <summary>
    /// Encrypted private key base64 representation
    /// </summary>
    /// <example>abc123...</example>
    public required string EncryptedPrivateKey { get; set; }

    /// <summary>
    /// Key salt for PBKDF2 key derivation
    /// </summary>
    /// <example>xyz789...</example>
    public required string Salt { get; set; }

    /// <summary>
    /// AES-GCM initialization vector
    /// </summary>
    /// <example>def456...</example>
    public required string Iv { get; set; }

    public CustomerKey ToEntity(int customerId)
    {
        return new CustomerKey
        {
            CustomerId = customerId,
            PublicKey = Convert.FromBase64String(PublicKey),
            EncryptedPrivateKey = Convert.FromBase64String(EncryptedPrivateKey),
            PrivateKeySalt = Convert.FromBase64String(Salt),
            Iv = Convert.FromBase64String(Iv),
        };
    }
}