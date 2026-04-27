#pragma warning disable CA1862
using System.Dynamic;
using System.Net.Mime;
using Asp.Versioning;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Common;
using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sorting;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("habits")]
[Authorize(Roles = Roles.Member)]
[ApiVersion(1.0)]
[Produces(
    MediaTypeNames.Application.Json,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.JsonV2,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1,
    CustomMediaTypeNames.Application.HateoasJsonV2)]
public sealed class HabitsController(
    ApplicationDbContext dbContext,
    LinkService linkService,
    UserContext userContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHabits(// we don't have a proper type to describe a data shaped response
        [FromQuery] HabitsQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService)
    {
        string? userId = await userContext.GetUserIdAsync();
        if(string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if(!sortMappingProvider.ValidateMappings<HabitDto, Habit>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter is not valid: '{query.Sort}'");
        }
        if (!dataShapingService.Validate<HabitDto>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields are not valid: '{query.Fields}'");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<HabitDto, Habit>();

        IQueryable<HabitDto> habitsQuery = dbContext.Habits
            .Where(h => h.UserId == userId)
            .Where(h => query.Search == null ||
                        h.Name.ToLower().Contains(query.Search) ||
                        h.Description != null && h.Description.ToLower().Contains(query.Search))
            .Where(h => query.Type == null || h.Type == query.Type)
            .Where(h => query.Status == null || h.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(HabitQueries.ProjectToDto());

        int totalCount = await habitsQuery.CountAsync();
        List<HabitDto> habits = await habitsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var paginationResult = new PaginationResult<ExpandoObject>
        {
            Items = dataShapingService.ShapeCollectionData(
                habits,
                query.Fields,
                query.IncludeLinks ? h => CreateLinksForHabit(h.Id, query.Fields) : null),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
        if (query.IncludeLinks)
        {
            paginationResult.Links = CreateLinksForHabits(query, paginationResult.HasNextPage, paginationResult.HasPreviousPage);
        }
        
        return Ok(paginationResult);
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetHabit(
        string id,
        DataShapingService dataShapingService,
        [FromHeader] HabitsQueryParameters query)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (!dataShapingService.Validate<HabitDto>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields are not valid: '{query.Fields}'");
        }

        HabitWithTagsDto? habit = await dbContext
            .Habits
            .Where(h => h.Id == id && h.UserId == userId)
            .Select(HabitQueries.ProjectToDtoWithTags())
            .FirstOrDefaultAsync();

        if(habit is null)
        {
            return NotFound();
        }

        ExpandoObject shapedObject = dataShapingService.ShapeData(habit, query.Fields);

        if (query.IncludeLinks)
        {
            List<LinkDto> links = CreateLinksForHabit(id, query.Fields);
            shapedObject.TryAdd("links", links);
        }

        return Ok(shapedObject);
    }

    [HttpGet("{id}")]
    [ApiVersion(2.0)]
    public async Task<IActionResult> GetHabitV2(
        string id,
        string? fields,
        DataShapingService dataShapingService,
        [FromHeader(Name = "Accept")] string? accept)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (!dataShapingService.Validate<HabitDto>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields are not valid: '{fields}'");
        }

        HabitWithTagsDtoV2? habit = await dbContext
            .Habits
            .Where(h => h.Id == id && h.UserId == userId)
            .Select(HabitQueries.ProjectToDtoWithTagsV2())
            .FirstOrDefaultAsync();

        if (habit is null)
        {
            return NotFound();
        }

        ExpandoObject shapedObject = dataShapingService.ShapeData(habit, fields);

        bool includeLinks = accept == CustomMediaTypeNames.Application.HateoasJson;

        if (includeLinks)
        {
            List<LinkDto> links = CreateLinksForHabit(id, fields);
            shapedObject.TryAdd("links", links);
        }

        return Ok(shapedObject);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(
        CreateHabitDto createHabitDto,
        IValidator<CreateHabitDto> validator)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await validator.ValidateAndThrowAsync(createHabitDto); // this will validate the dto and if invalid =>
                                                               // throws validation exception, which will be handled by
                                                               // the validator handler and return a problem details response

        Habit habit = createHabitDto.ToEntity(userId);

        dbContext.Habits.Add(habit);
        await dbContext.SaveChangesAsync();

        HabitDto habitDto = habit.ToDto();
        habitDto.Links = CreateLinksForHabit(habit.Id);

        return CreatedAtAction(nameof(GetHabit), new { id = habitDto.Id }, habitDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
        if(habit is null)
        {
            return NotFound();
        }
        habit.UpdateFromDto(updateHabitDto);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchHabit(string id, JsonPatchDocument<HabitDto> patchDocument)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
        if (habit is null)
        {
            return NotFound();
        }
        HabitDto habitDto = habit.ToDto();
        patchDocument.ApplyTo(habitDto, ModelState);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // support update only on name and description
        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description;
        habit.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
        if (habit is null)
        {
            return NotFound();
        }

        dbContext.Habits.Remove(habit);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private List<LinkDto> CreateLinksForHabits(HabitsQueryParameters query, bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkDto> links =
            [
                linkService.Create(nameof(GetHabits), "self", HttpMethods.Get, new
                {
                    q = query.Search,
                    type = query.Type,
                    status = query.Status,
                    sort = query.Sort,
                    fields = query.Fields,
                    page = query.Page,
                    pageSize = query.PageSize,
                }),
                linkService.Create(nameof(CreateHabit), "create", HttpMethods.Post)
            ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetHabits), "next-page", HttpMethods.Get, new
            {
                q = query.Search,
                type = query.Type,
                status = query.Status,
                sort = query.Sort,
                fields = query.Fields,
                page = query.Page + 1,
                pageSize = query.PageSize,
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(linkService.Create(nameof(GetHabits), "previous-page", HttpMethods.Get, new
            {
                q = query.Search,
                type = query.Type,
                status = query.Status,
                sort = query.Sort,
                fields = query.Fields,
                page = query.Page - 1,
                pageSize = query.PageSize,
            }));
        }

        return links;
    }

    private List<LinkDto> CreateLinksForHabit(string id, string? fields = null)
    {
        List<LinkDto> links =
            [
                linkService.Create(nameof(GetHabit), "self", HttpMethods.Get, new { id, fields }),
                linkService.Create(nameof(UpdateHabit), "update", HttpMethods.Put, new { id }),
                linkService.Create(nameof(PatchHabit), "partial-update", HttpMethods.Patch, new { id }),
                linkService.Create(nameof(DeleteHabit), "delete", HttpMethods.Delete, new { id }),
                linkService.Create(
                    nameof(HabitTagsController.UpsertHabitTags), 
                    "upsert-tags", 
                    HttpMethods.Put, 
                    new { habitId = id }, 
                    HabitTagsController.Name)
            ];
        return links;
    }
}
