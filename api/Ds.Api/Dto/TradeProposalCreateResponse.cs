using Ds.Core.Entities;

namespace Ds.Api.Dto;

public record TradeProposalCreateResponse
{
    public required int Id { get; set; }
    public required Metadata Metadata { get; set; }
    public required string MetadataRaw { get; set; }
    public required long CreatedAt { get; set; }
}

public static class TradeRecommendationExtensions
{
    public static TradeProposalCreateResponse ToTradeProposalCreateResponse(this TradeRecommendation recommendation) =>
        new()
        {
            Id = recommendation.Id,
            Metadata = recommendation.Metadata,
            MetadataRaw = recommendation.MetadataRaw,
            CreatedAt = recommendation.CreatedAt
        };
}