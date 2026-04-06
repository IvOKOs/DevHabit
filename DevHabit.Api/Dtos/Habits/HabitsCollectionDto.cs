#pragma warning disable S6964
using DevHabit.Api.Dtos.Common;

namespace DevHabit.Api.Dtos.Habits;

public sealed record HabitsCollectionDto : ICollectionResponse<HabitDto>
{
    public List<HabitDto> Items { get; init; }
}

