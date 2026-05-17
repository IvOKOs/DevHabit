using DevHabit.Api.Dtos.Auth;
using DevHabit.IntegrationTests.Infrastructure;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DevHabit.IntegrationTests.Tests;

public class AuthenticationTests(DevHabitWebAppFactory factory) : IntegrationTestFixture(factory)
{
    [Fact]
    public async Task Register_ShouldSucceed_WhenCorrectDtoInputIsPassed()
    {
        // Arrange
        RegisterUserDto userDto = new()
        {
            Name = "Test",
            Email = "test@devhabit.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(Routes.Auth.Register, userDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldReturnAccessTokens_WhenCorrectDtoInputIsPassed()
    {
        // Arrange
        RegisterUserDto userDto = new()
        {
            Name = "Test2",
            Email = "test2@devhabit.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(Routes.Auth.Register, userDto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert
        AccessTokensDto? accessTokens = await response.Content.ReadFromJsonAsync<AccessTokensDto>();
        Assert.NotNull(accessTokens);
    }
}
