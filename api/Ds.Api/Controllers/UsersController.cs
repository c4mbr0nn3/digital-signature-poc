using Ds.Api.Dto;
using Ds.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ds.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Tags("users")]
public class UsersController(ILogger<UsersController> logger, ICustomerKeyService customerKeyService) : ControllerBase
{
    /// <summary>
    /// Get active key for the current user
    /// </summary>
    /// <remarks>
    /// Retrieves the encrypted private key material for the user's currently active signing key
    /// </remarks>
    /// <response code="200">Active key retrieved successfully</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Access denied</response>
    /// <response code="404">No key found. Please perform onboarding process</response>
    [HttpGet("me/keys/active")]
    [ProducesResponseType<CustomerActiveKeyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Perform key onboarding process for the current user
    /// </summary>
    /// <remarks>
    /// Creates the user's first signing key. This should only be called once per user during initial setup.
    /// </remarks>
    /// <param name="request">Key onboarding request containing encrypted key material</param>
    /// <response code="200">Key onboarding performed successfully. Returns the new key ID.</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Access denied</response>
    [HttpPost("me/keys/onboarding")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    /// <summary>
    /// Perform key rotation/recovery process for the current user
    /// </summary>
    /// <remarks>
    /// Rotates to a new signing key while preserving the old key for historical signature verification
    /// </remarks>
    /// <param name="request">Key rotation request containing new encrypted key material</param>
    /// <response code="204">Key rotation performed successfully</response>
    /// <response code="401">Authentication required</response>
    /// <response code="403">Access denied</response>
    [HttpPost("me/keys/rotate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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