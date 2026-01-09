using System.Security.Claims;
using System.Text;
using AuthHW.Data;
using AuthHW.Entities;
using AuthHW.Services;
using AuthHW.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthHW.Extenssions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebComponents(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddGrpc();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Database connection string not found");
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        return services;
    }

    public static IServiceCollection AddAuthenticationWithJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenSettings = configuration.GetSection("TokenSettings").Get<TokenSettings>()
            ?? throw new InvalidOperationException("Token settings not configured");
        
        services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenSettings.Issuer,
                    ValidAudience = tokenSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(tokenSettings.SecretKey)),
                    
                    NameClaimType = ClaimTypes.NameIdentifier
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("JWT Failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("JWT Validated for user: " + context.Principal.Identity.Name);
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        
        services.AddScoped<AuthService>();
        services.AddScoped<UsersService>();
        services.AddScoped<ChatService>();
        
        return services;
    }
}