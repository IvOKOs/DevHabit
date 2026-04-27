using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Users;
using DevHabit.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DevHabit.Api.Services;

public sealed class UserContext(
    IHttpContextAccessor httpContextAccessor,//access the current http request
    ApplicationDbContext dbContext,
    IMemoryCache memoryCache)
{
    private const string CacheKeyPrefix = "users:id";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public async Task<string?> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        string? identityId = httpContextAccessor.HttpContext?.User.GetIdentityId();// reads identity id from the token
        if(identityId is null)// no user is logged in
        {
            return null;
        }

        string cacheKey = $"{CacheKeyPrefix}{identityId}";// unique name for the key for user id
        string? userId = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>// this is the cache factory => way of creating value if it is missing in cache
        {
            entry.SetSlidingExpiration(CacheDuration);// keep cache while used (if not used for 30 min, remove it)

            string? userId = await dbContext
            .Users
            .Where(u => u.IdentityId == identityId)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);// aloows operation to be stopped

            return userId;
        });
        return userId;
    }
}
