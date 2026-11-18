using Florarium.Api.Dtos.Dictionaries;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api")]
/// <summary>Справочники упаковки и лент для составления букета.</summary>
public class DictionariesController(DictionariesService service) : ControllerBase
{
    private readonly DictionariesService _service = service;

    [HttpGet("packagings")]
    /// <summary>Возвращает доступные варианты упаковки.</summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список упаковки с ценами.</returns>
    /// <response code="200">Справочник получен.</response>
    [ProducesResponseType(typeof(IReadOnlyList<PackagingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PackagingDto>>> GetPackagings(CancellationToken ct)
        => Ok(await _service.GetPackagingsAsync(ct));

    [HttpGet("ribbons")]
    /// <summary>Возвращает доступные варианты лент.</summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список лент с цветами.</returns>
    /// <response code="200">Справочник получен.</response>
    [ProducesResponseType(typeof(IReadOnlyList<RibbonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RibbonDto>>> GetRibbons(CancellationToken ct)
        => Ok(await _service.GetRibbonsAsync(ct));
}