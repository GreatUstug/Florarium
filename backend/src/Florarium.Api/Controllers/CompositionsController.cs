using Florarium.Api.Dtos.Compositions;
using Florarium.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/compositions")]
public class CompositionsController(CompositionsService service) : ControllerBase
{
    private readonly CompositionsService _service = service;

    [HttpPost("quote")]
    public async Task<ActionResult<QuoteResponse>> Quote(
        [FromBody] QuoteRequest request, 
        CancellationToken ct)
    {
        try
        {
            var response = await _service.QuoteAsync(request, ct);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message, Status = 400 });
        }
    }
}