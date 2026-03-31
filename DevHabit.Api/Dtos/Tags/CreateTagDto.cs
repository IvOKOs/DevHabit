namespace DevHabit.Api.Dtos.Tags;

public sealed record CreateTagDto
{
    public required string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
