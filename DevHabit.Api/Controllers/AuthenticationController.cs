using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Auth;
using DevHabit.Api.Dtos.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]// all action methods will be publically available
public sealed class AuthenticationController(
    UserManager<IdentityUser> userManager,// allows interaction with IdentityUsers table
    ApplicationDbContext applicationDbContext,// to save our users in db
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options
    ) : ControllerBase
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;


    [HttpPost("register")]
    public async Task<ActionResult<AccessTokensDto>> Register(RegisterUserDto registerUserDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        //create identity user
        var identityUser = new IdentityUser
        {
            UserName = registerUserDto.Name,
            Email = registerUserDto.Email,
        };

        IdentityResult createUserResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);
        if (!createUserResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                {
                    "errors",
                    createUserResult.Errors.ToDictionary(e => e.Code, e => e.Description)
                }
            };

            return Problem(
                detail: "Unable to register user. Please try again later.",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions);
        }

        IdentityResult addRoleResult = await userManager.AddToRoleAsync(identityUser, Roles.Member);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                {
                    "errors",
                    addRoleResult.Errors.ToDictionary(e => e.Code, e => e.Description)
                }
            };

            return Problem(
                detail: "Unable to register user. Please try again later.",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions);
        }

        //register user in identity table
        User registerUser = registerUserDto.ToEntity();
        registerUser.IdentityId = identityUser.Id;

        applicationDbContext.Users.Add(registerUser);
        await applicationDbContext.SaveChangesAsync();

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email, [Roles.Member]);
        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessTokens.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);
        await identityDbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok(accessTokens);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokensDto>> Login(LoginUserDto loginUserDto)
    {
        IdentityUser? identityUser = await userManager.FindByEmailAsync(loginUserDto.Email);

        if(identityUser is null || !await userManager.CheckPasswordAsync(identityUser, loginUserDto.Password))
        {
            return Unauthorized();
        }

        IEnumerable<string> userRoles = await userManager.GetRolesAsync(identityUser);

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!, userRoles);
        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessTokens.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);
        await identityDbContext.SaveChangesAsync();

        return Ok(accessTokens);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokensDto>> Refresh(RefreshTokenDto refreshTokenDto)
    {
        RefreshToken? refreshToken = await identityDbContext
            .RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.Token);

        if(refreshToken is null)
        {
            return Unauthorized();
        }

        if(refreshToken.ExpiresAtUtc <  DateTime.UtcNow)
        {
            return Unauthorized();
        }

        IEnumerable<string> userRoles = await userManager.GetRolesAsync(refreshToken.User);

        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!, userRoles);
        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessTokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);

        await identityDbContext.SaveChangesAsync();

        return Ok(accessTokens);
    }
}
