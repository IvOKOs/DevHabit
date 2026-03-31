namespace DevHabit.Api.Dtos.Tags;


public sealed record TagsCollectionDto
{
    public List<TagDto> Data { get; init; }
}

public sealed record TagDto
{
    public string Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
