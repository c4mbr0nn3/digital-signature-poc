using Ds.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ds.Api.Controllers;

[ApiController]
[Route("api/v1/trades")]
public class TradesController(
    ILogger<TradesController> logger,
    ITradeRecommendationService tradeRecommendationService) : ControllerBase
{
    [HttpPost("proposal")]
    public IActionResult CreateTradeRecommendation()
    {
        try
        {
            var recommendation = tradeRecommendationService.CreateRandomTradeRecommendation();
            return Ok(recommendation);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating trade recommendation: {Message}", e.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}