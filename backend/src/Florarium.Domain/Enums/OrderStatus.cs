namespace Florarium.Domain.Enums;

/// <summary>Этап жизненного цикла заказа.</summary>
public enum OrderStatus
{
    New, 
    Paid,
    Assembling,
    Delivering,
    Done,
    Cancelled
}