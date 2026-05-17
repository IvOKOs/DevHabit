using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using FluentValidation.TestHelper;
using System;
using System.Threading.Tasks;

namespace DevHabit.UnitTests.Validators;

public class CreateHabitDtoValidatorTests
{
    private readonly CreateHabitDtoValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldSucceedForName_WhenAllRulesAreMet()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Type = Api.Entities.HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = Api.Entities.FrequencyType.Weekly,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Name);
        //Assert.Empty(validationResult.Errors);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsEmpty()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = string.Empty,
            Type = Api.Entities.HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = Api.Entities.FrequencyType.Weekly,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsTooShort()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Do",
            Type = Api.Entities.HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = Api.Entities.FrequencyType.Weekly,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameExceedsMaxLength()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "MeasuringMeasuringMeasuringMeasuringMeasuringMeasuring",
            Type = Api.Entities.HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = Api.Entities.FrequencyType.Weekly,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = new string('b', 501),
            Type = Api.Entities.HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = Api.Entities.FrequencyType.Weekly,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTypeIsInvalid()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = (HabitType)3,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Weekly,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenFrequencyTypeIsInvalid()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = (FrequencyType)4,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Frequency.Type);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenFrequencyTimesPerPeriodIsZero()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 0,
            },
            Target = new TargetDto
            {
                Value = 3,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Frequency.TimesPerPeriod);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTargetValueIsZero()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 0,
                Unit = "pages"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Target.Value);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTargetUnitIsEmpty()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = string.Empty
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Target.Unit);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTargetUnitIsInvalid()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = "blocks"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Target.Unit);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenBinaryHabitHasInvalidUnit()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = "km"
            }
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Target.Unit);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenEndDateIsInPast()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = "sessions"
            },
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30))
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenMilestoneTargetIsZero()
    {
        // Arrange
        CreateHabitDto createHabitDto = new CreateHabitDto
        {
            Name = "Coding",
            Description = "descr",
            Type = HabitType.Binary,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 30,
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = "sessions"
            },
            MilestoneTarget = 0
        };

        // Act
        TestValidationResult<CreateHabitDto>? validationResult = await _validator.TestValidateAsync(createHabitDto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.MilestoneTarget);
    }
}
