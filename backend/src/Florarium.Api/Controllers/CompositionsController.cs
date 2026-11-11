using System.Security.Claims;

using Florarium.Api.Dtos.Compositions;
using Florarium.Api.Exceptions;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CompositionDetailDto>> Create([FromBody] SaveCompositionRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var result = await _service.CreateAsync(request, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new ProblemDetails
            {
                Title = ex.Message,
                Status = ex.StatusCode,
                Extensions = { ["code"] = ex.Code }
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompositionDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        Guid? userId = null;
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdStr, out var parsedId)) userId = parsedId;

        var result = await _service.GetByIdAsync(id, userId, ct);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CompositionDetailDto>> Update(Guid id, [FromBody] SaveCompositionRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var result = await _service.UpdateAsync(id, request, userId, ct);
            return Ok(result);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new ProblemDetails
            {
                Title = ex.Message,
                Status = ex.StatusCode,
                Extensions = { ["code"] = ex.Code }
            }); 
        }
    }
}