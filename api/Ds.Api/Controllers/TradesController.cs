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
}