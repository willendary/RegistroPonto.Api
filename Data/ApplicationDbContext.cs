
using Microsoft.EntityFrameworkCore;
using RegistroPonto.Api.Models;

namespace RegistroPonto.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TimeRecord> TimeRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Configure TimeRecord entity
        modelBuilder.Entity<TimeRecord>(entity =>
        {
            entity.HasOne(tr => tr.User)
                  .WithMany(u => u.TimeRecords)
                  .HasForeignKey(tr => tr.UserId);
        });
    }
}
