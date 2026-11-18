using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Cart;

/// <summary>Запрос на добавление состава букета в корзину.</summary>
public class AddToCartRequest
{
    /// <summary>Идентификатор состава букета.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required]
    /// <summary>Свойство CompositionId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid CompositionId { get; set; }
    
    /// <summary>Количество букетов от 1 до 50.</summary>
    /// <example>1</example>
    [Range(1, 50)]
    /// <summary>Свойство Qty.</summary>
    /// <example>1</example>
    public int Qty { get; set; } = 1;
}

/// <summary>Модель данных UpdateCartItemRequest.</summary>
public class UpdateCartItemRequest
{
    /// <summary>Новое количество букетов от 1 до 50.</summary>
    /// <example>2</example>
    [Range(1, 50)]
    /// <summary>Свойство Qty.</summary>
    /// <example>1</example>
    public int Qty { get; set; }
}
