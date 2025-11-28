using Ds.Core.Entities;
using Ds.Core.Enumerations;

namespace Ds.Api.Dto;

public record TradeProposalDetails
{
    public int Id { get; set; }
    public string MetadataRaw { get; set; }
    public Metadata Metadata { get; set; }
    public string Status { get; set; }
    public string Action { get; set; }
    public long CreatedAt { get; set; }
    public long? SignedAt { get; set; }
}

public static class TradeRecommendationDetailsExtensions
{
    public static TradeProposalDetails ToTradeProposalDetails(this TradeRecommendation recommendation) =>
        new()
        {
            Id = recommendation.Id,
            MetadataRaw = recommendation.MetadataRaw,
            Metadata = recommendation.Metadata,
            Status = recommendation.Status.ToLabel(),
            Action = recommendation.SignAction.ToLabel(),
            CreatedAt = recommendation.CreatedAt,
            SignedAt = recommendation.SignedAt
        };
}