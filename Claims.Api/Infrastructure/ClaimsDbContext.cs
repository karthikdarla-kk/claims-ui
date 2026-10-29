using Claims.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Api.Infrastructure;

public sealed class ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : DbContext(options)
{
    public DbSet<Claim> Claims => Set<Claim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var claim = modelBuilder.Entity<Claim>();
        claim.HasKey(item => item.Id);
        claim.HasIndex(item => item.ClaimNumber).IsUnique();
        claim.Property(item => item.ClaimNumber).HasMaxLength(30).IsRequired();
        claim.Property(item => item.ClaimantName).HasMaxLength(200).IsRequired();
        claim.Property(item => item.ClaimantEmail).HasMaxLength(320).IsRequired();
        claim.Property(item => item.Description).HasMaxLength(4000).IsRequired();
        claim.Property(item => item.AssignedOfficer).HasMaxLength(200);
        claim.Property(item => item.DecisionReason).HasMaxLength(2000);
        claim.Property(item => item.EstimatedLoss).HasPrecision(18, 2);
        claim.Property(item => item.Type).HasConversion<string>().HasMaxLength(30);
        claim.Property(item => item.Status).HasConversion<string>().HasMaxLength(40);
    }
}
