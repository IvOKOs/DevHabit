using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Users;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;


[ApiController]
[Authorize]// required authentication for all endpoints
[Route("users")]
public sealed class UsersController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
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
}
