using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos.Habits;

public static class HabitMapping
{
    public static HabitDto ToDto(this Habit habit)
    {
        return new HabitDto()
        {
            Id = habit.Id,
            Name = habit.Name,
            Description = habit.Description,
            Type = habit.Type,
            Frequency = new FrequencyDto()
            {
                Type = habit.Frequency.Type,
                TimesPerPeriod = habit.Frequency.TimesPerPeriod,
            },
            Target = new TargetDto()
            {
                Value = habit.Target.Value,
                Unit = habit.Target.Unit,
            },
            Status = habit.Status,
            IsArchived = habit.IsArchived,
            EndDate = habit.EndDate,
            MilestoneTarget = habit.MilestoneTarget,
            MilestoneCurrent = habit.MilestoneCurrent,
            CreatedAtUtc = habit.CreatedAtUtc,
            UpdatedAtUtc = habit.UpdatedAtUtc,
            LastCompletedAtUtc = habit.LastCompletedAtUtc,
        };
    }

    public static Habit ToEntity(this CreateHabitDto habit)
    {
        return new Habit()
        {
            Id = $"h_{Guid.CreateVersion7()}",
            Name = habit.Name,
            Description = habit.Description,
            Type = habit.Type,
            Frequency = new Frequency()
            {
                Type = habit.Frequency.Type,
                TimesPerPeriod = habit.Frequency.TimesPerPeriod,
            },
            Target = new Target()
            {
                Value = habit.Target.Value,
                Unit = habit.Target.Unit,
            },
            Status = HabitStatus.Ongoing,
            IsArchived = false,
            EndDate = habit.EndDate,
            MilestoneTarget = habit.MilestoneTarget,
            MilestoneCurrent = 0,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public static void UpdateFromDto(this Habit habit, UpdateHabitDto updateHabitDto)
    {
        habit.Name = updateHabitDto.Name;
        habit.Description = updateHabitDto.Description;
        habit.Type = updateHabitDto.Type;
        habit.Frequency = new Frequency
        {
            Type = updateHabitDto.Frequency.Type,
            TimesPerPeriod = updateHabitDto.Frequency.TimesPerPeriod,
        };
        habit.Target = new Target
        {
            Value = updateHabitDto.Target.Value,
            Unit = updateHabitDto.Target.Unit,
        };
        habit.EndDate = updateHabitDto.EndDate;
        habit.MilestoneTarget = updateHabitDto.MilestoneTarget;
        habit.UpdatedAtUtc = DateTime.UtcNow;
    }
}
