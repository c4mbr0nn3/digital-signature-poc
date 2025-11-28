using Ds.Api.Dto;
using Ds.Core.Entities;
using Ds.Core.Enumerations;

namespace Ds.Api.Extensions;

public static class TradeRecommendationExtensions
{
    extension(TradeRecommendation recommendation)
    {
        public TradeProposalDetails ToTradeProposalDetails() =>
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

        public TradeProposalCreateResponse ToTradeProposalCreateResponse() =>
            new()
            {
                Id = recommendation.Id,
                Metadata = recommendation.Metadata,
                MetadataRaw = recommendation.MetadataRaw,
                CreatedAt = recommendation.CreatedAt
            };
    }
}