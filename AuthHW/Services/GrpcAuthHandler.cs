using AuthHW.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace AuthHW.Services;

[Authorize]
public sealed class GrpcAuthHandler : Auth.AuthBase  
{
    private readonly AuthService _authService;

    public GrpcAuthHandler(AuthService authService)
    {
        _authService = authService;
    }

    public override async Task<RegisterReply> Register(
        RegisterRequest request,
        ServerCallContext context)
    {
        var result = await _authService.RegisterUserAsync(
            request.Login, 
            request.Password, 
            context.CancellationToken);

        return new RegisterReply
        {
            Success = result.Success,
            Message = result.Message ?? string.Empty
        };
    }

    public override async Task<LoginReply> Login(
        LoginRequest request,
        ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        var result = await _authService.AuthenticateUserAsync(
            request.Login,
            request.Password, 
            clientIp, 
            userAgent, 
            context.CancellationToken);

        return new LoginReply
        {
            Success = result.Success,
            Message = result.Message ?? string.Empty,
            AccessToken = result.Token ?? string.Empty,
            TokenType = result.Token != null ? "Bearer" : string.Empty,
            ExpiresInMinutes = result.ExpirationMinutes ?? 0
        };
    }
}