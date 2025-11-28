using Ds.Core.Entities;

namespace Ds.Api.Dto;

public record TradeProposalCreateResponse
{
    public required int Id { get; set; }
    public required Metadata Metadata { get; set; }
    public required string MetadataRaw { get; set; }
    public required long CreatedAt { get; set; }
}