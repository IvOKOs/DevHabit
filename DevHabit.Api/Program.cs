using DevHabit.Api;
using DevHabit.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
}

//use built-in exception handling middleware
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
