using Microsoft.AspNetCore.Mvc;
using BoardOutlook.Application.Interfaces;
using BoardOutlook.Application.DTOs.Response;

namespace BoardOutlook.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    [Produces("application/json")]
    public class ExecutivesController : ControllerBase
    {
        private readonly IExecutiveCompensationService _service;

        public ExecutivesController(IExecutiveCompensationService service)
        {
            _service = service;
        }

        [HttpGet(@"executives/compensation")]        
        public async Task<ActionResult<IEnumerable<ExecutiveCompensationResultDto>>> Get(
            [FromQuery] string exchange,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(exchange))
                return BadRequest("Exchange is required");

            var result = await _service.GetHighPaidExecutivesAsync(exchange, ct);
            return Ok(result);
        }
    }
}
