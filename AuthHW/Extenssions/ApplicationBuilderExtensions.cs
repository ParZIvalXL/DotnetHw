using AuthHW.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthHW.Extenssions;

public static class ApplicationBuilderExtensions
{
    public static void UseDevelopmentComponents(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    public static void UseSecurityComponents(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static async Task SetupDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}