using Ds.Api.Dto;
using Ds.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ds.Api.Controllers;

[ApiController]
[Route("api/v1/trades")]
[Tags("trades")]
public class TradesController(
    ILogger<TradesController> logger,
    ITradeRecommendationService tradeRecommendationService) : ControllerBase
{
    /// <summary>
    /// Get list of trade proposals
    /// </summary>
    /// <remarks>
    /// Retrieve the complete list of trade proposals for the current user
    /// </remarks>
    /// <response code="200">List retrieved successfully</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Access denied</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType<List<TradeProposalDetails>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<TradeProposalDetails>>> GetTradeRecommendations()
    {
        try
        {
            var result = await tradeRecommendationService.GetTradesRecommendations();
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching trade recommendations: {Message}", e.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new random trade proposal
    /// </summary>
    /// <remarks>
    /// Generate a new random trade proposal for testing purposes
    /// </remarks>
    /// <response code="200">Trade proposal created successfully</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Access denied</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("proposal")]
    [ProducesResponseType<TradeProposalCreateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TradeProposalCreateResponse>> CreateTradeRecommendation()
    {
        try
        {
            var result = await tradeRecommendationService.CreateRandomTradeRecommendation();
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating trade recommendation: {Message}", e.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Sign the trade proposal
    /// </summary>
    /// <remarks>
    /// Submit a cryptographic signature for a trade proposal. The signature must be created using the active signing key.
    /// </remarks>
    /// <param name="id">Unique identifier of trade to sign</param>
    /// <param name="request">Trade sign request containing signature and metadata</param>
    /// <response code="204">Trade proposal signed successfully</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Access denied</response>
    /// <response code="404">Trade proposal not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("{id:int}/sign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SignTradeRecommendation(int id, TradeSignRequest request)
    {
        try
        {
            await tradeRecommendationService.SignTradeRecommendation(id, request);
            return NoContent();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error signing trade recommendation: {Message}", e.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}