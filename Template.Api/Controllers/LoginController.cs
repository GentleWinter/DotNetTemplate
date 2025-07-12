using Microsoft.AspNetCore.Mvc;
using Template.Application.Services;
using Template.Domain.Entities;

namespace Template.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController (LoginService loginService)
    : ControllerBase
{
    public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] AuthInfo request)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(request.Email) && String.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Invalid client data.");

            var result = await loginService.AuthClient(request);

            return Ok( new { Token = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while login atempt: {ex.Message}");
        }
    }
        
    [HttpPost]
    [Route("GenerateAuthCode")]
    public async Task<ActionResult<bool>> GenerateAuthCode([FromBody] string email)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(email))
                return BadRequest("Invalid client data.");

            var result = await loginService.GenerateAuthCode(email);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while generating auth code atempt: {ex.Message}");
        }
    }
}