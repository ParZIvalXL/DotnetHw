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
    public async Task<IActionResult> Register([FromBody] RegisterCredentialsRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterUserAsync(request.Username, request.Tag, request.Email, request.Password, ct);
        

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
            request.TagOrEmail, request.Password, clientIp, userAgent, ct);

        if (!result.Success)
        {
            return Unauthorized(result.Message);
        }

        return Ok(new AuthenticationResult
        {
            AccessToken = result.Token!,
            ExpiresInMinutes = result.ExpirationMinutes!.Value,
            User = result.User
        });
    }
}