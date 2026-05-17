using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configurations;

public class HabitTagConfiguration : IEntityTypeConfiguration<HabitTag>
{
    public void Configure(EntityTypeBuilder<HabitTag> builder)
    {
        builder.HasKey(ht => new { ht.HabitId, ht.TagId });

        builder.HasOne<Habit>() // one habit
            .WithMany(h => h.HabitTags) // associated with many habittags and has a nav prop
            .HasForeignKey(ht => ht.HabitId);


        builder.HasOne<Tag>() // one tag
            .WithMany() // associated with many habittags
            .HasForeignKey(ht => ht.TagId);
    }
}
