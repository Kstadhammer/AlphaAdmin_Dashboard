using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<MemberEntity>(options)
{
    public virtual DbSet<ProjectEntity> Projects { get; set; } = null!;
    public virtual DbSet<ClientEntity> Clients { get; set; } = null!;
    public virtual DbSet<StatusEntity> Statuses { get; set; } = null!;
    public virtual DbSet<ProjectMemberEntity> ProjectMembers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship between Projects and Members
        modelBuilder
            .Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Project)
            .WithMany()
            .HasForeignKey(pm => pm.ProjectId);

        modelBuilder
            .Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Member)
            .WithMany()
            .HasForeignKey(pm => pm.MemberId);
    }
}
