using DevHabit.Api.Dtos.Entries;
using DevHabit.Api.Entities;
using FluentValidation.Results;
using System;
using System.Threading.Tasks;

namespace DevHabit.UnitTests.Validators;

public class CreateEntryDtoValidatorTests
{
    private readonly CreateEntryDtoValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSucceed_WhenInputDtoIsValid()
    {
        // Arrange
        var createEntryDto = new CreateEntryDto
        {
            HabitId = Habit.NewId(),
            Value = 1,
            Date = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(createEntryDto);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenHabitIdIsEmpty()
    {
        // Arrange
        var createEntryDto = new CreateEntryDto
        {
            HabitId = string.Empty,
            Value = 1,
            Date = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(createEntryDto);

        // Assert
        Assert.False(validationResult.IsValid);
        ValidationFailure validationFailure = Assert.Single(validationResult.Errors);
        Assert.Equal(nameof(createEntryDto.HabitId), validationFailure.PropertyName);
    }
}
