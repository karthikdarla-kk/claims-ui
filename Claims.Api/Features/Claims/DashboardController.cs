using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Features.Claims;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(ClaimService service) : ControllerBase
{
    [HttpGet]
    public Task<DashboardResponse> Get(CancellationToken cancellationToken) => service.GetDashboardAsync(cancellationToken);
}
