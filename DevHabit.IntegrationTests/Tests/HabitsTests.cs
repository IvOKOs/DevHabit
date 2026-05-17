using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using DevHabit.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;

namespace DevHabit.IntegrationTests.Tests;

public class HabitsTests(DevHabitWebAppFactory factory) : IntegrationTestFixture(factory)
{
    [Fact]
    public async Task CreateHabit_ShouldSucceed_ForValidDtoParameters()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Habit",
            Type = HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 30
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = "minutes"
            }
        };

        HttpClient httpClient = await CreateAuthenticatedClientAsync();

        // Act
        HttpResponseMessage? response = await httpClient.PostAsJsonAsync(Routes.Habits.Create, createHabitDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<HabitDto>());
    }
}
