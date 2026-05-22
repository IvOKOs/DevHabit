using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Common;
using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using DevHabit.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace DevHabit.IntegrationTests.Tests;

public class HabitsTests(DevHabitWebAppFactory factory) : IntegrationTestFixture(factory)
{
    [Fact]
    public async Task CreateHabit_ShouldSucceed_ForValidDtoParameters()
    {
        // Arrange
        await CleanupDatabaseAsync();
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

    [Fact]
    public async Task GetHabits_ShouldReturnAllHabits_IfHabitsExist()
    {
        // Arrange
        await CleanupDatabaseAsync();
        CreateHabitDto habitDto = TestData.Habits.CreateReadingHabit();
        HttpClient client = await CreateAuthenticatedClientAsync();

        await client.PostAsJsonAsync(Routes.Habits.Create, habitDto);

        // Act
        var response = await client.GetAsync(Routes.Habits.GetAll);

        // Assert
        PaginationResult<HabitDto>? result = await response.Content.ReadFromJsonAsync<PaginationResult<HabitDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(habitDto.Name, result.Items[0].Name);
    }

    [Fact]
    public async Task GetHabits_ShouldSupportFiltering()
    {
        // Arrange
        await CleanupDatabaseAsync();
        CreateHabitDto[] habits =
        {
            TestData.Habits.CreateReadingHabit(),
            TestData.Habits.CreateExerciseHabit(),
        };
        var client = await CreateAuthenticatedClientAsync();

        foreach(var habitDto in habits)
        {
            await client.PostAsJsonAsync(Routes.Habits.Create, habitDto);
        }

        // Act
        var response = await client.GetAsync($"{Routes.Habits.GetAll}?type={(int)HabitType.Binary}");

        // Assert
        PaginationResult<HabitDto>? result = await response.Content.ReadFromJsonAsync<PaginationResult<HabitDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(HabitType.Binary, result.Items[0].Type);
    }

    [Fact]
    public async Task GetHabits_ShouldSupportSorting()
    {
        // Arrange
        await CleanupDatabaseAsync();
        CreateHabitDto[] habits =
        {
            TestData.Habits.CreateReadingHabit(),
            TestData.Habits.CreateExerciseHabit(),
        };
        var client = await CreateAuthenticatedClientAsync();

        foreach (var habitDto in habits)
        {
            await client.PostAsJsonAsync(Routes.Habits.Create, habitDto);
        }
        const string SortParams = "name";

        // Act
        var response = await client.GetAsync($"{Routes.Habits.GetAll}?sort={SortParams}");

        // Assert
        PaginationResult<HabitDto>? result = await response.Content.ReadFromJsonAsync<PaginationResult<HabitDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Exercise", result.Items[0].Name);
        Assert.Equal("Reading a book", result.Items[1].Name);
    }

    [Fact]
    public async Task GetHabit_ShouldReturnHabit_WhenExists()
    {
        // Arrange
        await CleanupDatabaseAsync();
        CreateHabitDto createHabitDto = TestData.Habits.CreateExerciseHabit();
        HttpClient client = await CreateAuthenticatedClientAsync();

        var habitResponse = await client.PostAsJsonAsync(Routes.Habits.Create, createHabitDto);
        var createdHabitDto = await habitResponse.Content.ReadFromJsonAsync<HabitDto>();
        Assert.NotNull(createdHabitDto);

        // Act
        var response = await client.GetAsync($"{Routes.Habits.GetById(createdHabitDto.Id)}");

        // Assert
        HabitWithTagsDto? habitResult = await response.Content.ReadFromJsonAsync<HabitWithTagsDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(habitResult);
        Assert.Equal(createdHabitDto.Id, habitResult.Id);
    }
}
