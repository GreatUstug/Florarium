using Florarium.Api.Dtos.Dictionaries;
using Florarium.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api")]
public class DictionariesController(DictionariesService service) : ControllerBase
{
    private readonly DictionariesService _service = service;

    [HttpGet("packagings")]
    public async Task<ActionResult<IReadOnlyList<PackagingDto>>> GetPackagings(CancellationToken ct)
        => Ok(await _service.GetPackagingsAsync(ct));

    [HttpGet("ribbons")]
    public async Task<ActionResult<IReadOnlyList<RibbonDto>>> GetRibbons(CancellationToken ct)
        => Ok(await _service.GetRibbonsAsync(ct));
}