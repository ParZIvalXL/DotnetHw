using AuthHW.DTOs;
using AuthHW.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthHW.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class ApiAuthController : ControllerBase
{
    private readonly AuthService _authService;

    public ApiAuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CredentialsRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterUserAsync(request.Username, request.Password, ct);
        
        if (!result.Success)
        {
            return result.Message == "Username already taken" 
                ? Conflict(result.Message) 
                : BadRequest(result.Message);
        }

        return Created(string.Empty, new { Message = result.Message });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResult>> Login(
        [FromBody] CredentialsRequest request, 
        CancellationToken ct)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var result = await _authService.AuthenticateUserAsync(
            request.Username, request.Password, clientIp, userAgent, ct);

        if (!result.Success)
        {
            return Unauthorized(result.Message);
        }

        return Ok(new AuthenticationResult
        {
            AccessToken = result.Token!,
            ExpiresInMinutes = result.ExpirationMinutes!.Value
        });
    }
}