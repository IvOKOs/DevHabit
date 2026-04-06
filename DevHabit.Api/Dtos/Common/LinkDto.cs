namespace DevHabit.Api.Dtos.Common;

public sealed record LinkDto
{
    public string Href { get; init; }
    public string Rel {  get; init; }
    public string Method { get; init; }
}
