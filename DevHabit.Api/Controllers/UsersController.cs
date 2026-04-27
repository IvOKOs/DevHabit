using System.Security.Claims;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;


[ApiController]
[Authorize(Roles = Roles.Member)]// required authentication + authorization for all endpoints
[Route("users")]
public sealed class UsersController(ApplicationDbContext dbContext, UserContext userContext) : ControllerBase
{
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if(id != userId)// without this, any user could get another user's data => data leak
        {
            return Forbid();
        }

        UserDto? userDto = await dbContext
            .Users
            .Where(u => u.Id == id)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if(userDto is null)
        {
            return NotFound();
        }

        return Ok(userDto);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        UserDto? userDto = await dbContext
            .Users
            .Where(u => u.Id == userId)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (userDto is null)
        {
            return NotFound();
        }

        return Ok(userDto);
    }
}
