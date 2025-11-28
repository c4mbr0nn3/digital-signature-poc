using Ds.Core.Entities;

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