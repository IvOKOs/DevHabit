using DevHabit.Api.Extensions;
using DevHabit.Api.Middleware;
using DevHabit.Api.Settings;
using static DevHabit.Api.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddBackgroundJobs()
    .AddCorsPolicy();



WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
    await app.SeedInitialDataAsync();
}

//use built-in exception handling middleware
app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ETagMiddleware>();

app.MapControllers();

await app.RunAsync();

public partial class Program;
