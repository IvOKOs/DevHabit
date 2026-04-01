using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Tags;
using DevHabit.Api.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("tags")]
public class TagsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TagsCollectionDto>> GetTags()
    {
        List<TagDto> tags = await dbContext
            .Tags
            .Select(TagQueries.ProjectToDto())
            .ToListAsync();

        var tagsCollection = new TagsCollectionDto
        {
            Data = tags
        };
        return Ok(tagsCollection);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
        TagDto? tagDto = await dbContext
            .Tags
            .Where(t => t.Id == id)
            .Select(TagQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if(tagDto is null)
        {
            return NotFound();
        }

        return Ok(tagDto);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(
        CreateTagDto createTagDto,
        IValidator<CreateTagDto> validator)
    {
        ValidationResult validationResult = await validator.ValidateAsync(createTagDto);
        if (!validationResult.IsValid)
        {
            //return BadRequest(validationResult.ToDictionary()); // will serialize the validation error to dictionary, but it doesn't return problem details response
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary())); // it returns problem details response but it is not complete
        }

        if(await dbContext.Tags.AnyAsync(t => t.Name == createTagDto.Name))
        {
            return Problem(
                detail: $"A tag with the given name '{createTagDto.Name}' already exists.",
                statusCode: StatusCodes.Status409Conflict);
        }

        Tag tag = createTagDto.ToEntity();

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        TagDto tagDto = tag.ToDto();

        return CreatedAtAction(nameof(GetTag), new { id = tagDto.Id }, tagDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTag(string id, UpdateTagDto updateTagDto)
    {
        Tag? tag = await dbContext.Tags.FindAsync(id);
        if(tag is null)
        {
            return NotFound();
        }

        tag.UpdateFromDto(updateTagDto);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id)
    {
        Tag? tag = await dbContext.Tags.FindAsync(id);
        if(tag is null)
        {
            return NotFound();
        }

        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
