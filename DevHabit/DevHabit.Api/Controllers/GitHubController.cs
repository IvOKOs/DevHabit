using DevHabit.Api.Dtos.Common;
using DevHabit.Api.Dtos.GitHub;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Controllers;

[ApiController]
[Authorize(Roles = Roles.Member)]
[Route("github")]
public sealed class GitHubController(
    GitHubAccessTokenService gitHubAccessTokenService,
    GitHubService gitHubService,// communicates with the github api
    UserContext userContext,// to get info about curr user
    LinkService linkService// for hateoas implementation
    ) : ControllerBase
{
    //stores the access token for curr user -> upsert functionality
    [HttpPut("personal-access-token")]
    public async Task<IActionResult> StoreAccessToken(StoreGitHubAccessTokenDto storeGitHubAccessTokenDto)
    {
        string? userId = await userContext.GetUserIdAsync();
        if(string.IsNullOrWhiteSpace(userId) )
        {
            return Unauthorized();
        }

        await gitHubAccessTokenService.StoreAsync(userId, storeGitHubAccessTokenDto);
        return NoContent();
    }

    [HttpDelete("personal-access-token")]
    public async Task<IActionResult> RevokeAccessToken()
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await gitHubAccessTokenService.RevokeAsync(userId);
        return NoContent();
    }

    [HttpGet("profile")]
    public async Task<ActionResult<GitHubUserProfileDto>> GetUserProfile([FromHeader] AcceptHeaderDto acceptHeaderDto)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        string? gitHubAccessToken = await gitHubAccessTokenService.GetAsync(userId);
        if (string.IsNullOrEmpty(gitHubAccessToken))
        {
            return NotFound();
        }

        GitHubUserProfileDto? userProfileDto = await gitHubService.GetUserProfileAsync(gitHubAccessToken);
        if(userProfileDto is null)
        {
            return NotFound();
        }

        if (acceptHeaderDto.IncludeLinks)
        {
            userProfileDto.Links =
            [
                linkService.Create(nameof(GetUserProfile), "self", HttpMethods.Get),
                linkService.Create(nameof(StoreAccessToken), "store-token", HttpMethods.Put),
                linkService.Create(nameof(RevokeAccessToken), "revoke-token", HttpMethods.Delete)
            ];
        }

        return Ok(userProfileDto);
    }

    [HttpGet("events")]
    public async Task<ActionResult<IReadOnlyList<GitHubEventDto>>> GetUserEvents()
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        string? accessToken = await gitHubAccessTokenService.GetAsync(userId);
        if (accessToken is null)
        {
            return Unauthorized();
        }

        GitHubUserProfileDto? profile = await gitHubService.GetUserProfileAsync(accessToken);

        if (profile is null)
        {
            return NotFound();
        }

        IReadOnlyList<GitHubEventDto>? events = await gitHubService.GetUserEventsAsync(
            profile.Login,
            accessToken);

        if (events is null)
        {
            return NotFound();
        }

        return Ok(events);
    }
}
