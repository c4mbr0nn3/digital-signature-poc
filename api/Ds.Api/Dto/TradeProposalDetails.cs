using Ds.Core.Entities;

namespace Ds.Api.Dto;

/// <summary>
/// Complete details of a trade proposal
/// </summary>
public record TradeProposalDetails
{
    /// <summary>
    /// Unique identifier for the trade proposal
    /// </summary>
    /// <example>123</example>
    public int Id { get; set; }

    /// <summary>
    /// Raw metadata string for signing purposes
    /// </summary>
    /// <example>{"trades":[{"isin":"LU0290358497","qty":10,"price":34.56,"ccy":"EUR"}]}</example>
    public string MetadataRaw { get; set; }

    /// <summary>
    /// Parsed metadata object containing trade details
    /// </summary>
    public Metadata Metadata { get; set; }

    /// <summary>
    /// Status of the trade proposal
    /// </summary>
    /// <example>pending</example>
    public string Status { get; set; }

    /// <summary>
    /// Action taken from the customer on the trade proposal
    /// </summary>
    /// <example>accepted</example>
    public string Action { get; set; }

    /// <summary>
    /// Creation timestamp in unix timestamp milliseconds format
    /// </summary>
    /// <example>1733140200000</example>
    public long CreatedAt { get; set; }

    /// <summary>
    /// Signing timestamp in unix timestamp milliseconds format
    /// </summary>
    /// <example>1733140200000</example>
    public long? SignedAt { get; set; }
}