using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Ds.Core.Entities;

[Table("trade_recommendations")]
[Index(nameof(CustomerId))]
public class TradeRecommendation
{
    [Column("id"), Key] public int Id { get; set; }
    [Column("customer_id")] public int CustomerId { get; set; }
    [Column("metadata")] public required string MetadataRaw { get; set; }

    [Column("signed_action"), MaxLength(10)]
    public string? SignedAction { get; set; }

    [Column("signed_at")] public long? SignedAt { get; set; }
    [Column("signing_key_id")] public int? SigningKeyId { get; set; }
    [Column("created_at")] public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    [ForeignKey(nameof(CustomerId))] public virtual Customer? Customer { get; set; }
    [ForeignKey(nameof(SigningKeyId))] public virtual CustomerKey? SigningKey { get; set; }

    [NotMapped]
    public Metadata Metadata
    {
        get => JsonSerializer.Deserialize<Metadata>(MetadataRaw) ?? new Metadata();
        set => MetadataRaw = JsonSerializer.Serialize(value);
    }
}

public class Metadata
{
    [JsonPropertyName("trades")] public List<TradeData> Trades { get; set; } = [];
}

public class TradeData
{
    [JsonPropertyName("isin")] public string Isin { get; set; } = string.Empty;
    [JsonPropertyName("qty")] public int Quantity { get; set; }
    [JsonPropertyName("price")] public decimal Price { get; set; }
    [JsonPropertyName("ccy")] public string Currency { get; set; } = string.Empty;
}