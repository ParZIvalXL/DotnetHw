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

    public async Task<TokenResult> RegisterUserAsync(string username, string tag, string email, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(password))
        {
            return new TokenResult { Success = false, Message = "Username and password required" };
        }
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new TokenResult { Success = false, Message = "Username and password required" };
        }

        var normalizedUsername = tag.ToUpperInvariant();
        var userExists = await _context.UserAccounts
            .AnyAsync(u => u.Tag == normalizedUsername || u.Email == email, ct);

        if (userExists)
        {
            throw new ArgumentException("Такой пользователь уже существует");
        }

        var newUser = new UserAccount
        {
            Username = username,
            Email = email,
            Tag = tag,
            PasswordHash = _passwordHasher.HashPassword(null!, password)
        };

        var newProfile = new UserProfile()
        {
            DisplayName = username,
            UserId = newUser.Id,
            User = newUser,
            UpdatedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow
            
        };
        
        _context.UserAccounts.Add(newUser);
        _context.UserProfiles.Add(newProfile);
        
        
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

        var user = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username, ct);

        var isValid = user != null && 
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) 
            != PasswordVerificationResult.Failed;

        await LogAuthenticationAttemptAsync(
            username, isValid, clientIp, userAgent, ct);

        if (!isValid || user == null)
        {
            return new TokenResult { Success = false, Message = "Invalid credentials" };
        }

        var token = _tokenGenerator.CreateToken(
            user.Username,
            user.Id,
            _tokenSettings.Issuer,
            _tokenSettings.Audience,
            _tokenSettings.SecretKey,
            _tokenSettings.TokenValidityMinutes);

        return new TokenResult
        {
            Success = true,
            Message = "Authentication successful",
            Token = token,
            ExpirationMinutes = _tokenSettings.TokenValidityMinutes,
            User = user
        };
    }

    private async Task LogAuthenticationAttemptAsync(
        string username,
        bool wasSuccessful,
        string? clientIp,
        string? userAgent,
        CancellationToken ct)
    {
        var attempt = new AuthAttempt
        {
            Username = username,
            WasSuccessful = wasSuccessful,
            ClientIp = clientIp,
            UserAgent = userAgent
        };

        _context.AuthAttempts.Add(attempt);
        await _context.SaveChangesAsync(ct);
    }
}