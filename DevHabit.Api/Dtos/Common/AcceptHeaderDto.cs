using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using DevHabit.Api.Services;

namespace DevHabit.Api.Dtos.Common;

public record AcceptHeaderDto
{
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }

    public bool IncludeLinks =>
        MediaTypeHeaderValue.TryParse(Accept, out MediaTypeHeaderValue? mediaType) &&
        mediaType.MediaType!.Contains("hateoas", StringComparison.OrdinalIgnoreCase);
}
