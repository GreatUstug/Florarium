namespace Florarium.Api.Dtos.Cart;

/// <summary>Содержимое корзины и итоговые суммы.</summary>
public class CartResponse
{
    /// <summary>Позиции корзины.</summary>
    public List<CartItemDto> Items { get; set; } = [];
    /// <summary>Общее количество единиц товаров.</summary>
    /// <example>3</example>
    /// <summary>Свойство Count.</summary>
    /// <example>1</example>
    public int Count { get; set; }
    /// <summary>Сумма до скидок.</summary>
    /// <example>4500.00</example>
    /// <summary>Свойство Subtotal.</summary>
    /// <example>100.00</example>
    public decimal Subtotal { get; set; }
    /// <summary>Итоговая сумма корзины.</summary>
    /// <example>4500.00</example>
    /// <summary>Свойство Total.</summary>
    /// <example>100.00</example>
    public decimal Total { get; set; }
    /// <summary>Дата окончания резерва остатков.</summary>
    /// <example>2026-11-18T12:30:00Z</example>
    /// <summary>Свойство ReservedUntil.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    public DateTime? ReservedUntil { get; set; }
}

/// <summary>Модель данных CartItemDto.</summary>
public class CartItemDto
{
    /// <summary>Идентификатор позиции корзины.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Данные состава букета.</summary>
    public CartCompositionDto Composition { get; set; } = null!;
    /// <summary>Количество букетов.</summary>
    /// <example>2</example>
    /// <summary>Свойство Qty.</summary>
    /// <example>1</example>
    public int Qty { get; set; }
    /// <summary>Цена одного букета.</summary>
    /// <example>2250.00</example>
    /// <summary>Свойство UnitPrice.</summary>
    /// <example>100.00</example>
    public decimal UnitPrice { get; set; }
    /// <summary>Стоимость строки позиции.</summary>
    /// <example>4500.00</example>
    /// <summary>Свойство LineTotal.</summary>
    /// <example>100.00</example>
    public decimal LineTotal { get; set; }
    /// <summary>Состояние резерва позиции.</summary>
    /// <example>reserved</example>
    /// <summary>Свойство Status.</summary>
    /// <example>пример</example>
    public string Status { get; set; } = "reserved";
    /// <summary>Максимально доступное количество.</summary>
    /// <example>10</example>
    /// <summary>Свойство MaxQty.</summary>
    /// <example>1</example>
    public int MaxQty { get; set; } = 50;
}

/// <summary>Модель данных CartCompositionDto.</summary>
public class CartCompositionDto
{
    /// <summary>Идентификатор состава.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Название букета.</summary>
    /// <example>Романтический букет</example>
    /// <summary>Свойство Title.</summary>
    /// <example>пример</example>
    public string? Title { get; set; }
    /// <summary>URL фотографии букета.</summary>
    /// <example>https://example.com/bouquet.jpg</example>
    /// <summary>Свойство PhotoUrl.</summary>
    /// <example>пример</example>
    public string? PhotoUrl { get; set; }
    /// <summary>Источник состава: готовый или пользовательский.</summary>
    /// <example>catalog</example>
    /// <summary>Свойство Source.</summary>
    /// <example>пример</example>
    public string Source { get; set; } = null!;
}
