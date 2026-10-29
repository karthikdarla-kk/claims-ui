using Claims.Api.Domain;
using Claims.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Claims.Api.Features.Claims;

public sealed class ClaimService(ClaimsDbContext db)
{
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync(string? email = null, ClaimStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = db.Claims.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(claim => claim.ClaimantEmail == email);
        if (status.HasValue)
            query = query.Where(claim => claim.Status == status.Value);

        return await query.OrderByDescending(claim => claim.CreatedAt).ToListAsync(cancellationToken);
    }

    public Task<Claim?> GetClaimAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Claims.AsNoTracking().SingleOrDefaultAsync(claim => claim.Id == id, cancellationToken);

    public async Task<Claim> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);
        var claim = new Claim
        {
            ClaimNumber = $"CLM-{DateTime.UtcNow:yyyy}-{await GetNextClaimNumberAsync(cancellationToken):D4}",
            ClaimantName = request.ClaimantName.Trim(),
            ClaimantEmail = request.ClaimantEmail.Trim(),
            Type = request.Type,
            IncidentDate = request.IncidentDate,
            Description = request.Description.Trim(),
            EstimatedLoss = request.EstimatedLoss
        };

        db.Claims.Add(claim);
        await db.SaveChangesAsync(cancellationToken);
        return claim;
    }

    public async Task<Claim?> UpdateAsync(Guid id, UpdateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var claim = await db.Claims.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (claim is null) return null;

        if (request.Status is ClaimStatus.Settled or ClaimStatus.Rejected && string.IsNullOrWhiteSpace(request.DecisionReason))
            throw new ArgumentException("A decision reason is required when settling or rejecting a claim.");
        if (request.Status == ClaimStatus.InReview && string.IsNullOrWhiteSpace(request.AssignedOfficer))
            throw new ArgumentException("An officer must be assigned when a claim enters review.");

        claim.Status = request.Status;
        claim.DecisionReason = request.DecisionReason?.Trim();
        claim.AssignedOfficer = request.AssignedOfficer?.Trim();
        claim.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return claim;
    }

    public async Task<DashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var claims = await db.Claims.AsNoTracking().ToListAsync(cancellationToken);
        var counts = Enum.GetValues<ClaimStatus>().ToDictionary(status => status, status => claims.Count(claim => claim.Status == status));
        return new DashboardResponse(
            claims.Count,
            claims.Count(claim => claim.Status is not (ClaimStatus.Settled or ClaimStatus.Rejected)),
            claims.Where(claim => claim.Status is not (ClaimStatus.Settled or ClaimStatus.Rejected)).Sum(claim => claim.EstimatedLoss),
            counts);
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await db.Claims.AnyAsync(cancellationToken)) return;

        db.Claims.AddRange(
            new Claim
            {
                ClaimNumber = "CLM-2026-0001", ClaimantName = "Aisha Tan", ClaimantEmail = "aisha@example.com",
                Type = ClaimType.Motor, IncidentDate = new DateOnly(2026, 2, 14),
                Description = "Rear-end collision at a traffic light.", EstimatedLoss = 4200m,
                Status = ClaimStatus.InReview, AssignedOfficer = "M. Chen",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-2), UpdatedAt = DateTimeOffset.UtcNow.AddHours(-4)
            },
            new Claim
            {
                ClaimNumber = "CLM-2026-0002", ClaimantName = "Daniel Wong", ClaimantEmail = "daniel@example.com",
                Type = ClaimType.Property, IncidentDate = new DateOnly(2026, 2, 20),
                Description = "Storm damage to roof and water ingress.", EstimatedLoss = 18500m,
                Status = ClaimStatus.Submitted, CreatedAt = DateTimeOffset.UtcNow.AddDays(-1), UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1)
            });
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> GetNextClaimNumberAsync(CancellationToken cancellationToken)
    {
        var count = await db.Claims.CountAsync(cancellationToken);
        return count + 1;
    }

    private static void ValidateCreateRequest(CreateClaimRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClaimantName) || string.IsNullOrWhiteSpace(request.ClaimantEmail))
            throw new ArgumentException("Claimant name and email are required.");
        if (request.EstimatedLoss < 0)
            throw new ArgumentException("Estimated loss cannot be negative.");
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required.");
    }
}