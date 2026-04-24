using DevHabit.Api.Dtos.Common;
using DevHabit.Api.Dtos.Habits;

namespace DevHabit.Api.Dtos.Tags;

public sealed record TagDto : ILinksResponse
{
    public string Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public List<LinkDto> Links { get; set; }
}
