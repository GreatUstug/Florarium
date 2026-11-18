namespace Florarium.Api.Dtos.Compositions;

/// <summary>Модель данных QuoteResponse.</summary>
public class QuoteResponse
{
    /// <summary>Свойство Items.</summary>
    /// <example>[]</example>
    public List<QuoteItemResponse> Items { get; set; } = [];
    /// <summary>Свойство FlowersTotal.</summary>
    /// <example>100.00</example>
    public decimal FlowersTotal { get; set; }
    /// <summary>Свойство PackagingPrice.</summary>
    /// <example>100.00</example>
    public decimal PackagingPrice { get; set; }
    /// <summary>Свойство Total.</summary>
    /// <example>100.00</example>
    public decimal Total { get; set; }
    /// <summary>Свойство Warnings.</summary>
    /// <example>[]</example>
    public List<QuoteWarningDto> Warnings { get; set; } = [];
}

/// <summary>Модель данных QuoteItemResponse.</summary>
public class QuoteItemResponse
{
    /// <summary>Свойство FlowerId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid FlowerId { get; set; }
    /// <summary>Свойство FlowerName.</summary>
    /// <example>пример</example>
    public string FlowerName { get; set; } = null!;
    /// <summary>Свойство Qty.</summary>
    /// <example>1</example>
    public int Qty { get; set; }
    /// <summary>Свойство PricePerStem.</summary>
    /// <example>100.00</example>
    public decimal PricePerStem { get; set; }
    /// <summary>Свойство LineTotal.</summary>
    /// <example>100.00</example>
    public decimal LineTotal { get; set; }
}

/// <summary>Модель данных QuoteWarningDto.</summary>
public class QuoteWarningDto
{
    /// <summary>Свойство Code.</summary>
    /// <example>пример</example>
    public string Code { get; set; } = "out_of_stock";
    /// <summary>Свойство FlowerId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid FlowerId { get; set; }
    /// <summary>Свойство Requested.</summary>
    /// <example>1</example>
    public int Requested { get; set; }
    /// <summary>Свойство Available.</summary>
    /// <example>1</example>
    public int Available { get; set; }
}
