using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;

namespace DevHabit.IntegrationTests.Infrastructure;

public static class TestData
{
    public static class Habits
    {
        public static CreateHabitDto CreateReadingHabit() => new()
        {
            Name = "Reading a book",
            Frequency = new FrequencyDto
            {
                TimesPerPeriod = 30,
                Type = FrequencyType.Daily,
            },
            Target = new TargetDto
            {
                Value = 60,
                Unit = "minutes"
            },
            Type = HabitType.Measurable,

        };

        public static CreateHabitDto CreateExerciseHabit() => new()
        {
            Name = "Exercise",
            Frequency = new FrequencyDto
            {
                TimesPerPeriod = 1,
                Type = FrequencyType.Weekly,
            },
            Target = new TargetDto
            {
                Value = 90,
                Unit = "sessions"
            },
            Type = HabitType.Binary,
        };
    }
}
