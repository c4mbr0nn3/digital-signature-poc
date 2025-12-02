namespace Ds.Api.Dto;

/// <summary>
/// Active customer signing key material
/// </summary>
public class CustomerActiveKeyResponse
{
    /// <summary>
    /// Unique identifier for the key
    /// </summary>
    /// <example>123</example>
    public int Id { get; set; }

    /// <summary>
    /// Encrypted Private Key base64 representation
    /// </summary>
    /// <example>abc123...</example>
    public string EncryptedPrivateKey { get; set; }

    /// <summary>
    /// Key salt for PBKDF2 key derivation
    /// </summary>
    /// <example>xyz789...</example>
    public string Salt { get; set; }

    /// <summary>
    /// AES-GCM initialization vector
    /// </summary>
    /// <example>def456...</example>
    public string Iv { get; set; }
}