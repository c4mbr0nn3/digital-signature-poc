using Ds.Api.Dto;
using Ds.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ds.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(ILogger<UsersController> logger, ICustomerKeyService customerKeyService) : ControllerBase
{
    [HttpGet("me/keys/active")]
    public async Task<ActionResult<CustomerActiveKeyResponse>> GetActiveUserKey()
    {
        try
        {
            var result = await customerKeyService.GetActiveUserKey();
            if (result == null) return NotFound("Active key not found for current user.");
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching active key for current user: {Message}", e.Message);
            throw;
        }
    }

    [HttpPost("me/keys/onboarding")]
    public async Task<IActionResult> OnboardCustomerKey(CustomerKeyOnboardingRequest request)
    {
        try
        {
            var keyId = await customerKeyService.OnboardCustomerKey(request);
            return Ok(keyId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error onboarding customer key for current user: {Message}", e.Message);
            throw;
        }
    }

    [HttpPost("me/keys/rotate")]
    public async Task<IActionResult> RotateCustomerKey(CustomerKeyOnboardingRequest request)
    {
        try
        {
            await customerKeyService.RotateCustomerKey(request);
            return NoContent();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error rotating customer key for current user: {Message}", e.Message);
            throw;
        }
    }
}