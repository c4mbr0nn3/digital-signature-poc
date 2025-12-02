using Ds.Core.Entities;

namespace Ds.Api.Dto;

/// <summary>
/// Response after creating a new trade proposal
/// </summary>
public record TradeProposalCreateResponse
{
    /// <summary>
    /// Unique identifier for the trade proposal
    /// </summary>
    /// <example>123</example>
    public required int Id { get; set; }

    /// <summary>
    /// Parsed metadata object containing trade details
    /// </summary>
    public required Metadata Metadata { get; set; }

    /// <summary>
    /// Raw metadata string for signing purposes
    /// </summary>
    /// <example>{"trades":[{"isin":"LU0290358497","qty":10,"price":34.56,"ccy":"EUR"}]}</example>
    public required string MetadataRaw { get; set; }

    /// <summary>
    /// Unix timestamp in milliseconds
    /// </summary>
    /// <example>1733140200000</example>
    public required long CreatedAt { get; set; }
}