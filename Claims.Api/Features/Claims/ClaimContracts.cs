using Claims.Api.Domain;

namespace Claims.Api.Features.Claims;

public sealed record CreateClaimRequest(
    string ClaimantName,
    string ClaimantEmail,
    ClaimType Type,
    DateOnly IncidentDate,
    string Description,
    decimal EstimatedLoss);

public sealed record UpdateClaimRequest(
    ClaimStatus Status,
    string? DecisionReason,
    string? AssignedOfficer);

public sealed record ClaimResponse(
    Guid Id,
    string ClaimNumber,
    string ClaimantName,
    string ClaimantEmail,
    ClaimType Type,
    DateOnly IncidentDate,
    string Description,
    decimal EstimatedLoss,
    ClaimStatus Status,
    string? AssignedOfficer,
    string? DecisionReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static ClaimResponse From(Claim claim) => new(
        claim.Id,
        claim.ClaimNumber,
        claim.ClaimantName,
        claim.ClaimantEmail,
        claim.Type,
        claim.IncidentDate,
        claim.Description,
        claim.EstimatedLoss,
        claim.Status,
        claim.AssignedOfficer,
        claim.DecisionReason,
        claim.CreatedAt,
        claim.UpdatedAt);
}

public sealed record DashboardResponse(
    int TotalClaims,
    int OpenClaims,
    decimal OutstandingExposure,
    IReadOnlyDictionary<ClaimStatus, int> ClaimsByStatus);
