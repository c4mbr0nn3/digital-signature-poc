using System.Text.Json.Serialization;

namespace Ds.Api.Dto;

public record TradeSignRequest
{
    public required int TradeId { get; set; } // redundant, for convenience
    public required string Signature { get; set; } // Base64 encoded
    public required int SigningKeyId { get; set; }
    public required string SignedAction { get; set; }
    public required long SignedAt { get; set; }

    [JsonIgnore] public byte[] SignatureBytes => Convert.FromBase64String(Signature);
}