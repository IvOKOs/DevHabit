using DevHabit.Api.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    { // ensures database schema is up-to-date by applying unapplied migrations
        using IServiceScope scope = app.Services.CreateScope();
        
        await using ApplicationDbContext applicationDbContext = 
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await using ApplicationIdentityDbContext identityDbContext = 
            scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        {
            await applicationDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application database migrations applied successfully.");

            await identityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occured while applying database migrations.");
            throw;
        }
    }
}
