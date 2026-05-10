using DevHabit.Api.Database;
using DevHabit.Api.Dtos.GitHub;
using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Services;

public sealed class GitHubAccessTokenService(ApplicationDbContext dbContext)//to save and revoke the access token for the user
{
    public async Task StoreAsync(
        string userId,
        StoreGitHubAccessTokenDto accessTokenDto,
        CancellationToken cancellationToken = default)
    {
        GitHubAccessToken? existingAccessToken = await GetAccessTokenAsync(userId, cancellationToken);

        if(existingAccessToken is not null)// if it exists, overwrite
        {
            existingAccessToken.Token = accessTokenDto.AccessToken;
            existingAccessToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(accessTokenDto.ExpiresInDays);
        }
        else// if not, create and store in db
        {
            dbContext.GitHubAccessTokens.Add(new GitHubAccessToken
            {
                Id = $"gh_{Guid.CreateVersion7()}",
                UserId = userId,
                Token = accessTokenDto.AccessToken,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(accessTokenDto.ExpiresInDays)
            });
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string?> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        GitHubAccessToken? gitHubAccessToken = await GetAccessTokenAsync(userId, cancellationToken);

        return gitHubAccessToken?.Token;
    }

    public async Task RevokeAsync(string userId, CancellationToken cancellationToken = default)
    {// deletes the token from the db
        GitHubAccessToken? gitHubAccessToken = await GetAccessTokenAsync(userId, cancellationToken);

        if (gitHubAccessToken is null)
        {
            return;
        }

        dbContext.GitHubAccessTokens.Remove(gitHubAccessToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<GitHubAccessToken?> GetAccessTokenAsync(string userId, CancellationToken cancellationToken)
    {
        return await dbContext.GitHubAccessTokens.SingleOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }
}
