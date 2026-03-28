using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;

namespace DevHabit.Api.Entities;

public sealed class Habit
{
    public string Id { get; set; } // string => I want to construct id with a prefix
    public string Name { get; set; }
    public string? Description { get; set; }
    public HabitType Type { get; set; }
    public Frequency Frequency { get; set; }
    public Target Target { get; set; }
    public HabitStatus Status { get; set; } // for completion status of a habit
    public bool IsArchived { get; set; } // for data archiving
    public DateOnly? EndDate { get; set; }
    public int? MilestoneTarget { get; set; }
    public int? MilestoneCurrent { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? LastCompletedAtUtc { get; set; } // when last completed a specific habit that has attached milestone

    [NotMapped]
    public Milestone Milestone => new Milestone
    {
        Target = new Target { Value = MilestoneTarget ?? 0 },
        Current = new Target { Value = MilestoneCurrent ?? 0 }
    };
}

public enum HabitType
{
    None,
    Binary, // complete a specific task or not
    Measurable // track values
}

public enum HabitStatus
{
    None,
    Ongoing,
    Completed
}

public class Frequency
{
    public FrequencyType Type { get; set; } // count how often to perform a habit
    public int TimesPerPeriod { get; set; }
}

public enum FrequencyType
{
    None,
    Daily,
    Weekly,
    Monthly
}

public class Target
{
    public int Value { get; set; }
    public string Unit { get; set; }
}

public class Milestone
{// when these two values are equal => goal is completed
    public Target Target { get; set; }
    public Target Current { get; set; }
}



