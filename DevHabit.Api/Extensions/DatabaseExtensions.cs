using DevHabit.Api.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    { // ensures database schema is up-to-date by applying unapplied migrations
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int retries = 10;
        while (true)
        {
            try
            {
                await app.ApplyMigrationsAsync();
                break;
            }
            catch (SqlException ex)
            {
                if (retries-- == 0)
                {
                    throw;
                }
                app.Logger.LogInformation(ex,"Waiting for SQL Server...");
                await Task.Delay(5000);
            }
        }
        //try
        //{
        //    await dbContext.Database.MigrateAsync();
        //    app.Logger.LogInformation("Database migrations applied successfully.");
        //}
        //catch (Exception ex)
        //{
        //    app.Logger.LogError(ex, "An error occured while applying database migrations.");
        //    throw;
        //}
    }
}
