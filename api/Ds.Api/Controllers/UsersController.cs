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
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching active key for current user: {Message}", e.Message);
            throw;
        }
    }
}