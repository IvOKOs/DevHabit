using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Auth;
using DevHabit.Api.Dtos.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]// all action methods will be publically available
public sealed class AuthenticationController(
    UserManager<IdentityUser> userManager,// allows interaction with IdentityUsers table
    ApplicationDbContext applicationDbContext,// to save our users in db
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider
    ) : ControllerBase
{
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

        IdentityResult identityResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);
        if (!identityResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                {
                    "errors",
                    identityResult.Errors.ToDictionary(e => e.Code, e => e.Description)
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

        await transaction.CommitAsync();

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email);
        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

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

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!);
        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

        return Ok(accessTokens);
    }
}
