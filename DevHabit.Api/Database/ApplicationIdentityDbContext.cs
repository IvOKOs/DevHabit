using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Database;

public sealed class ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
    : IdentityDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)// configures the base identity tables
    {
        base.OnModelCreating(builder);// this wires up the identity tables

        builder.HasDefaultSchema(Schemas.Identity);

        // to customize identity table names
        builder.Entity<IdentityUser>().ToTable("asp_net_users");// snake case naming convention
        builder.Entity<IdentityRole>().ToTable("asp_net_roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("asp_net_user_roles");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("asp_net_role_claims");
        builder.Entity<IdentityUserClaim<string>>().ToTable("asp_net_user_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("asp_net_user_logins");// for external login
        builder.Entity<IdentityUserToken<string>>().ToTable("asp_net_user_tokens");

    }
}
