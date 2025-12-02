using Ds.Api.Dto;
using Ds.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ds.Api.Controllers;

[ApiController]
[Route("api/v1/trades")]
public class TradesController(
    ILogger<TradesController> logger,
    ITradeRecommendationService tradeRecommendationService) : ControllerBase
{
    [HttpGet]
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

    [HttpPost("proposal")]
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

    [HttpPost("{id:int}/sign")]
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