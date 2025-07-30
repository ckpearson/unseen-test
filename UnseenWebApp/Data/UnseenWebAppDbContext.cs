using System;
using Microsoft.EntityFrameworkCore;

namespace UnseenWebApp.Data;

public class UnseenWebAppDbContext : DbContext
{
    public UnseenWebAppDbContext(DbContextOptions<UnseenWebAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TopScoreUniqueStringEntity> TopScoreUniqueStrings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TopScoreUniqueStringEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Word)
                .IsRequired();

            entity.Property(e => e.SubmittedAtUtc)
                .IsRequired();

            entity.HasIndex(e => e.Word)
                .IsUnique();

            entity.HasIndex(e => e.SubmittedAtUtc)
                .IsDescending();
        });
    }
}
