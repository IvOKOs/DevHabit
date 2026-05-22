using DevHabit.Api.Dtos.GitHub;
using DevHabit.IntegrationTests.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace DevHabit.IntegrationTests.Tests;

public class GitHubTests(DevHabitWebAppFactory factory) : IntegrationTestFixture(factory)
{
    private const string TestAccessToken = "gho_123456789test";

    // to represent the response from the user endpoint
    private static readonly GitHubUserProfileDto User = new(
        Login: "testuser",
        Name: "Test User",
        AvatarUrl: "https://github.com/testuser.png",
        Bio: "Test bio",
        PublicRepos: 10,
        Followers: 20,
        Following: 30
    );

    // to represent the response from events endpoint
    private static readonly GitHubEventDto TestEvent = new(
        Id: "1234567890",
        Type: "PushEvent",
        Actor: new GitHubActorDto(
            Id: 1,
            Login: "testuser",
            DisplayLogin: "testuser",
            AvatarUrl: "https://github.com/testuser.png"
        ),
        Repository: new GitHubRepositoryDto(
            Id: 1,
            Name: "testuser/repo",
            Url: "https://api.github.com/repos/testuser/repo"
        ),
        Payload: new GitHubPayloadDto(
            Action: "test-action",
            Ref: "refs/heads/main",
            Commits:
            [
                new GitHubCommitDto(
                    Sha: "abc123",
                    Message: "Test commit",
                    Url: "https://github.com/testuser/repo/commit/abc123"
                )
            ]
        ),
        IsPublic: true,
        CreatedAt: DateTime.Parse("2025-01-01T00:00:00Z", CultureInfo.InvariantCulture)
    );

    [Fact]
    public async Task GetUserProfile_ShouldReturnCorrectProfile_WhenInputIsCorrect()
    {
        // Arrange
        WireMockServer
            .Given(Request.Create()
                .WithPath("/user")
                .WithHeader("Authorization", $"Bearer {TestAccessToken}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", MediaTypeNames.Application.Json)
                .WithBodyAsJson(User));

        HttpClient client = await CreateAuthenticatedClientAsync();

        StoreGitHubAccessTokenDto accessTokenDto = new()
        { 
            AccessToken = TestAccessToken, 
            ExpiresInDays = 30 
        };

        await client.PutAsJsonAsync(Routes.GitHub.StoreAccessToken, accessTokenDto);

        // Act
        HttpResponseMessage response = await client.GetAsync(Routes.GitHub.GetProfile);
        response.EnsureSuccessStatusCode();

        // Assert
        var profile = JsonConvert.DeserializeObject<GitHubUserProfileDto>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(profile);
        Assert.Equivalent(User, profile);
    }
}
