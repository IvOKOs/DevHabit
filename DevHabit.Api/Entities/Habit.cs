using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;

namespace DevHabit.Api.Entities;

public sealed class Habit
{
    public string Id { get; set; } // string => I want to construct id with a prefix
    public string UserId { get; set; }
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
    
    public List<HabitTag> HabitTags { get; set; }
    public List<Tag> Tags { get; set; }
    
}

//public class Milestone
//{// when these two values are equal => goal is completed
//    public int Target { get; set; }
//    public int Current { get; set; }
//}



