using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ds.Core.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Ds.Core.Entities;

[Table("trade_recommendations")]
[Index(nameof(CustomerId))]
public class TradeRecommendation
{
    [Column("id"), Key] public int Id { get; set; }
    [Column("customer_id")] public int CustomerId { get; set; }
    [Column("metadata")] public required string MetadataRaw { get; set; }
    [Column("signature")] public byte[]? Signature { get; set; }
    [Column("signed_action")] public int SignedAction { get; set; } = (int)SignAction.Pending;
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

    [NotMapped]
    public SignAction SignAction
    {
        get => (SignAction)SignedAction;
        set => SignedAction = (int)value;
    }

    [NotMapped] public TradeStatus Status => SignedAt.HasValue ? TradeStatus.Signed : TradeStatus.Pending;
}

/// <summary>
/// Trade recommendation metadata containing list of trades
/// </summary>
public class Metadata
{
    [JsonPropertyName("trades")] public List<TradeData> Trades { get; set; } = [];
}

/// <summary>
/// Individual trade item details
/// </summary>
public class TradeData
{
    /// <summary>
    /// Item ISIN identifier
    /// </summary>
    /// <example>LU0290358497</example>
    [JsonPropertyName("isin")]
    public string Isin { get; set; } = string.Empty;

    /// <summary>
    /// Item quantity
    /// </summary>
    /// <example>10</example>
    [JsonPropertyName("qty")]
    public int Quantity { get; set; }

    /// <summary>
    /// Item price
    /// </summary>
    /// <example>34.56</example>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Item currency (3-letter code)
    /// </summary>
    /// <example>EUR</example>
    [JsonPropertyName("ccy")]
    public string Currency { get; set; } = string.Empty;
}