using DevHabit.Api.Database;
using DevHabit.Api.Dtos.HabitTags;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Authorize(Roles = Roles.Member)]
[Route("habits/{habitId}/tags")]
public class HabitTagsController(ApplicationDbContext dbContext) : ControllerBase
{
    public static readonly string Name = nameof(HabitTagsController).Replace("Controller", string.Empty);

    [HttpPut]
    public async Task<ActionResult> UpsertHabitTags(string habitId, UpsertHabitTagsDto upsertHabitTagsDto)
    {
        Habit? habit = await dbContext
            .Habits
            .Include(h => h.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId);

        if(habit is null)
        {
            return NotFound();
        }

        // GOOD -> filter on db level before loading into memory
        List<string> existingTagIds = await dbContext
            .Tags
            .Where(t => upsertHabitTagsDto.TagIds.Contains(t.Id))
            .Select(h => h.Id)
            .ToListAsync();
        if(existingTagIds.Count != upsertHabitTagsDto.TagIds.Count)
        {
            return BadRequest("One or more tag IDs is invalid.");
        }

        // check if all tags of the habit are exactly the ones passed to request
        var currentTagIds = habit.HabitTags.Select(ht => ht.TagId).ToHashSet();
        if (currentTagIds.SetEquals(upsertHabitTagsDto.TagIds))
        {
            return NoContent();
        }

        habit.HabitTags.RemoveAll(ht => !upsertHabitTagsDto.TagIds.Contains(ht.TagId));
        var tagIdsToAdd = upsertHabitTagsDto.TagIds.Except(habit.HabitTags.Select(ht => ht.TagId)).ToList();
        habit.HabitTags.AddRange(tagIdsToAdd.Select(tagId => new HabitTag
        {
            HabitId = habit.Id,
            TagId = tagId,
            CreatedAtUtc = DateTime.UtcNow,
        }));

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{tagId}")]
    public async Task<ActionResult> RemoveTagFromHabit(string habitId, string tagId)
    {
        HabitTag? tagToRemove = await dbContext
            .HabitTags
            .SingleOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);// should be only one => if duplicates exist => exception

        if(tagToRemove is null)
        {
            return NotFound();
        }

        dbContext.HabitTags.Remove(tagToRemove);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
