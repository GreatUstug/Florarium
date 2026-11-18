using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/delivery")]
public class DeliveryController : ControllerBase
{
/// <summary>Получение доступных временных интервалов доставки.</summary>
    /// <summary>Возвращает стандартные интервалы доставки для даты.</summary>
    /// <param name="date">Дата доставки; если не указана, используется текущая дата UTC.</param>
    /// <returns>Дата и доступные временные слоты.</returns>
    /// <response code="200">Слоты успешно получены.</response>
    [HttpGet("slots")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSlots([FromQuery] DateOnly? date)
    {
        var slots = new[]
        {
            new { slot = "10:00–13:00", available = true },
            new { slot = "13:00–16:00", available = true },
            new { slot = "16:00–19:00", available = true },
            new { slot = "19:00–22:00", available = true }
        };

        return Ok(new { date = date?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd"), slots });
    }
}