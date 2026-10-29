using Claims.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Features.Claims;

[ApiController]
[Route("api/claims")]
public sealed class ClaimsController(ClaimService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClaimResponse>>> GetAll([FromQuery] string? email, [FromQuery] ClaimStatus? status, CancellationToken cancellationToken) =>
        Ok((await service.GetClaimsAsync(email, status, cancellationToken)).Select(ClaimResponse.From));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var claim = await service.GetClaimAsync(id, cancellationToken);
        return claim is null ? NotFound() : Ok(ClaimResponse.From(claim));
    }

    [HttpPost]
    public async Task<ActionResult<ClaimResponse>> Create(CreateClaimRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var claim = await service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = claim.Id }, ClaimResponse.From(claim));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ClaimResponse>> Update(Guid id, UpdateClaimRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var claim = await service.UpdateAsync(id, request, cancellationToken);
            return claim is null ? NotFound() : Ok(ClaimResponse.From(claim));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
