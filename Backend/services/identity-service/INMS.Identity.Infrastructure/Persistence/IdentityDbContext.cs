using Microsoft.EntityFrameworkCore;
using INMS.Identity.Domain.Entities;

namespace INMS.Identity.Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserAreaAssignment> UserAreaAssignments { get; set; }
    public DbSet<AccountRequest> AccountRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Role>().ToTable("Role");
        modelBuilder.Entity<UserAreaAssignment>().ToTable("UserAreaAssignment");
        modelBuilder.Entity<AccountRequest>().ToTable("AccountRequest");

        base.OnModelCreating(modelBuilder);
    }
}
