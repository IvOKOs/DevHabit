using DevHabit.Api;
using DevHabit.Api.Database;
using DevHabit.Api.Extensions;
using DevHabit.Api.Middleware;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using static DevHabit.Api.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices();



WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
}

//use built-in exception handling middleware
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
