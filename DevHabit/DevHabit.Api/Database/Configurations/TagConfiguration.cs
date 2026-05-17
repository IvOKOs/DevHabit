using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configurations;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasMaxLength(500);

        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();

        builder.Property(t => t.Description).HasMaxLength(500);

        //builder.HasIndex(t => new { t.Name }).IsUnique(); // not allowed to create a duplicate tag
        //tag name should be scoped to each user
        builder.HasIndex(t => new { t.UserId, t.Name }).IsUnique();// tag names are unique for each user

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
