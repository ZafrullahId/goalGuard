using Microsoft.EntityFrameworkCore;
using goalGuard.Entity;
using goalGuard.Data.Configurations;

namespace goalGuard.Data;

public class GoalGuardDbContext : DbContext
{
    public GoalGuardDbContext(DbContextOptions<GoalGuardDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
