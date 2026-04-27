using DevHabit.Api.Database;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Identity;
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

    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        RoleManager<IdentityRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            if(!await roleManager.RoleExistsAsync(Roles.Member))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Member));
            }
            if(!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            app.Logger.LogInformation("Successfully created roles.");
        }
        catch(Exception ex)
        {
            app.Logger.LogError(ex, "An error occured while seeding initial data.");
            throw;
        }
    }
}
