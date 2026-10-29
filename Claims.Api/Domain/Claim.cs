namespace Claims.Api.Domain;

public sealed class Claim
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string ClaimNumber { get; init; } = string.Empty;
    public string ClaimantName { get; set; } = string.Empty;
    public string ClaimantEmail { get; set; } = string.Empty;
    public ClaimType Type { get; set; }
    public DateOnly IncidentDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedLoss { get; set; }
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    public string? AssignedOfficer { get; set; }
    public string? DecisionReason { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public enum ClaimType
{
    Motor,
    Property
}

public enum ClaimStatus
{
    Submitted,
    InReview,
    AwaitingInformation,
    Settled,
    Rejected
}
