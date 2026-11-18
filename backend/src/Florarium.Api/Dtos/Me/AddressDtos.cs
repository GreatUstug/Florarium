using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Me;

/// <summary>Сохраненный адрес доставки.</summary>
public class AddressDto
{
    /// <summary>Идентификатор адреса.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Пользовательская метка адреса.</summary>
    /// <example>Дом</example>
    /// <summary>Свойство Label.</summary>
    /// <example>пример</example>
    public string? Label { get; set; }
    /// <summary>Улица и номер дома.</summary>
    /// <example>ул. Ленина, д. 10</example>
    /// <summary>Свойство Street.</summary>
    /// <example>пример</example>
    public string Street { get; set; } = null!;
    /// <summary>Квартира или офис.</summary>
    /// <example>42</example>
    /// <summary>Свойство Apartment.</summary>
    /// <example>пример</example>
    public string? Apartment { get; set; }
    /// <summary>Подъезд.</summary>
    /// <example>2</example>
    /// <summary>Свойство Entrance.</summary>
    /// <example>пример</example>
    public string? Entrance { get; set; }
    /// <summary>Этаж.</summary>
    /// <example>5</example>
    /// <summary>Свойство Floor.</summary>
    /// <example>пример</example>
    public string? Floor { get; set; }
    /// <summary>Домофон.</summary>
    /// <example>42К1</example>
    /// <summary>Свойство Intercom.</summary>
    /// <example>пример</example>
    public string? Intercom { get; set; }
}

/// <summary>Модель данных CreateAddressRequest.</summary>
public class CreateAddressRequest
{
    /// <summary>Пользовательская метка адреса.</summary>
    /// <example>Дом</example>
    [MaxLength(50)]
    /// <summary>Свойство Label.</summary>
    /// <example>пример</example>
    public string? Label { get; set; }
    
    [Required, MaxLength(300)]
    /// <summary>Свойство Street.</summary>
    /// <example>пример</example>
    public string Street { get; set; } = null!;
    
    [MaxLength(20)]
    /// <summary>Свойство Apartment.</summary>
    /// <example>пример</example>
    public string? Apartment { get; set; }
    
    [MaxLength(20)]
    /// <summary>Свойство Entrance.</summary>
    /// <example>пример</example>
    public string? Entrance { get; set; }
    
    [MaxLength(20)]
    /// <summary>Свойство Floor.</summary>
    /// <example>пример</example>
    public string? Floor { get; set; }
    
    [MaxLength(20)]
    /// <summary>Свойство Intercom.</summary>
    /// <example>пример</example>
    public string? Intercom { get; set; }
}
