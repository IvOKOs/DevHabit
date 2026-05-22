using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Auth;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WireMock.Server;

namespace DevHabit.IntegrationTests.Infrastructure;

// one instance shared across multiple test classes
//[Collection(nameof(IntegrationTestCollection))]
public abstract class IntegrationTestFixture(DevHabitWebAppFactory factory) : IClassFixture<DevHabitWebAppFactory> // one instance of test container(DevHabitWebAppFactory) created for each test class => multiple test classes = multiple test containers
{
    private HttpClient? _authorizedClient;
    public HttpClient CreateClient() => factory.CreateClient();
    public WireMockServer WireMockServer => factory.GetWireMockServer();


    protected async Task CleanupDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        string? connectionString = configuration.GetConnectionString("Database");
        if (connectionString is null)
        {
            throw new InvalidOperationException("Database connection string not found in configuration");
        }

        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = new(@"
                -- Delete children first
                DELETE FROM devhabit.Entries;
                DELETE FROM devhabit.Tags;
    
                -- Then parents
                DELETE FROM devhabit.Habits;
                DELETE FROM devhabit.Users;
                DELETE FROM [identity].RefreshTokens;
                DELETE FROM [identity].asp_net_users;
            ", connection);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(
        string email = "test@test.com",
        string password = "Test123!")
    {
        if (_authorizedClient is not null)
        {
            return _authorizedClient;
        }

        HttpClient client = CreateClient();

        // Check if user exists
        bool userExists;
        using (IServiceScope scope = factory.Services.CreateScope())
        {
            using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            userExists = await dbContext.Users.AnyAsync(u => u.Email == email);
        }

        if (!userExists)
        {
            // Register a new user
            HttpResponseMessage registerResponse = await client.PostAsJsonAsync(Routes.Auth.Register,
                new RegisterUserDto
                {
                    Email = email,
                    Name = email,
                    Password = password,
                    ConfirmPassword = password
                });

            registerResponse.EnsureSuccessStatusCode();
        }

        // Login to get the token
        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(Routes.Auth.Login,
            new LoginUserDto
            {
                Email = email,
                Password = password
            });

        loginResponse.EnsureSuccessStatusCode();

        AccessTokensDto? loginResult = await loginResponse.Content.ReadFromJsonAsync<AccessTokensDto>();

        if (loginResult?.AccessToken is null)
        {
            throw new InvalidOperationException("Failed to get authentication token");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        _authorizedClient = client;

        return client;
    }
}
