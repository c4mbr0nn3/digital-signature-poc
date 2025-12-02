using System.Text.Json.Serialization;

namespace Ds.Api.Dto;

/// <summary>
/// Request to sign a trade proposal
/// </summary>
public record TradeSignRequest
{
    /// <summary>
    /// ID of the trade to be signed (redundant, for convenience)
    /// </summary>
    /// <example>123</example>
    public required int TradeId { get; set; }

    /// <summary>
    /// Signature in base64 format
    /// </summary>
    /// <example>MEUCIQDx5...</example>
    public required string Signature { get; set; }

    /// <summary>
    /// Signing key ID
    /// </summary>
    /// <example>456</example>
    public required int SigningKeyId { get; set; }

    /// <summary>
    /// Signed action (accepted or rejected)
    /// </summary>
    /// <example>accepted</example>
    public required string SignedAction { get; set; }

    /// <summary>
    /// Unix timestamp in milliseconds
    /// </summary>
    /// <example>1733140200000</example>
    public required long SignedAt { get; set; }

    [JsonIgnore] public byte[] SignatureBytes => Convert.FromBase64String(Signature);
}