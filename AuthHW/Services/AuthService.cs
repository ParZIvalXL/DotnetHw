using AuthHW.Data;
using AuthHW.DTOs;
using AuthHW.Entities;
using AuthHW.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthHW.Services;

public sealed class AuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly TokenSettings _tokenSettings;

    public AuthService(
        AppDbContext context,
        IPasswordHasher<UserAccount> passwordHasher,
        ITokenGenerator tokenGenerator,
        IOptions<TokenSettings> tokenSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _tokenSettings = tokenSettings.Value;
    }

    public async Task<TokenResult> RegisterUserAsync(string username, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new TokenResult { Success = false, Message = "Username and password required" };
        }

        var normalizedUsername = username.ToUpperInvariant();
        var userExists = await _context.UserAccounts
            .AnyAsync(u => u.NormalizedUsername == normalizedUsername, ct);

        if (userExists)
        {
            return new TokenResult { Success = false, Message = "Username already taken" };
        }

        var newUser = new UserAccount
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            PasswordHash = _passwordHasher.HashPassword(null!, password)
        };

        _context.UserAccounts.Add(newUser);
        await _context.SaveChangesAsync(ct);

        return new TokenResult { Success = true, Message = "Registration successful" };
    }

    public async Task<TokenResult> AuthenticateUserAsync(
        string username,
        string password,
        string? clientIp,
        string? userAgent,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new TokenResult { Success = false, Message = "Credentials required" };
        }

        var normalizedUsername = username.ToUpperInvariant();
        var user = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.NormalizedUsername == normalizedUsername, ct);

        var isValid = user != null && 
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) 
            != PasswordVerificationResult.Failed;

        await LogAuthenticationAttemptAsync(
            username, normalizedUsername, isValid, clientIp, userAgent, ct);

        if (!isValid || user == null)
        {
            return new TokenResult { Success = false, Message = "Invalid credentials" };
        }

        var token = _tokenGenerator.CreateToken(
            user.Username,
            _tokenSettings.Issuer,
            _tokenSettings.Audience,
            _tokenSettings.SecretKey,
            _tokenSettings.TokenValidityMinutes);

        return new TokenResult
        {
            Success = true,
            Message = "Authentication successful",
            Token = token,
            ExpirationMinutes = _tokenSettings.TokenValidityMinutes
        };
    }

    private async Task LogAuthenticationAttemptAsync(
        string username,
        string normalizedUsername,
        bool wasSuccessful,
        string? clientIp,
        string? userAgent,
        CancellationToken ct)
    {
        var attempt = new AuthAttempt
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            WasSuccessful = wasSuccessful,
            ClientIp = clientIp,
            UserAgent = userAgent
        };

        _context.AuthAttempts.Add(attempt);
        await _context.SaveChangesAsync(ct);
    }
}