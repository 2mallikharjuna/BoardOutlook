using Microsoft.AspNetCore.Mvc;
using BoardOutlook.Application.Interfaces;
using BoardOutlook.Application.DTOs.Response;

namespace BoardOutlook.Api.Controllers
{
    /// <summary>
    /// Controller to manage executive compensation endpoints.
    /// </summary>
    [ApiController]
    [Route("api/companies")]
    [Produces("application/json")]
    public class ExecutivesController : ControllerBase
    {
        private readonly IExecutiveCompensationService _service;

        /// <summary>
        /// Constructor injecting the Executive Compensation service.
        /// </summary>
        /// <param name="service">Service to fetch and process executive compensation data.</param>
        public ExecutivesController(IExecutiveCompensationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves executives whose total compensation is at least 10% above the industry average
        /// for all companies listed in the specified exchange.
        /// </summary>
        /// <param name="exchange">Stock exchange code (e.g., "ASX").</param>
        /// <param name="ct">Cancellation token to cancel the request.</param>
        /// <returns>List of executives above industry compensation threshold.</returns>
        /// <response code="200">Returns the list of high-paid executives.</response>
        /// <response code="400">If the exchange query parameter is missing or empty.</response>
        [HttpGet("executives/compensation")]
        public async Task<ActionResult<IEnumerable<ExecutiveCompensationResultDto>>> Get(
            [FromQuery] string exchange,
            CancellationToken ct)
        {
            try
            {
                // Validate query parameter
                if (string.IsNullOrWhiteSpace(exchange))
                    return BadRequest("Exchange is required");

                // Fetch high-paid executives from the service
                var result = await _service.GetHighPaidExecutivesAsync(exchange, ct);

                // Return results with HTTP 200 OK
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                // Return HTTP 500 Internal Server Error with exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
