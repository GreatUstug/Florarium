namespace Florarium.Domain.Enums;

/// <summary>Состояние исполнения доставки.</summary>
public enum DeliveryStatus
{
    Pending,
    InTransit,
    Delivered,
    Failed
}