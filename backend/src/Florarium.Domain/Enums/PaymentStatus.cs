namespace Florarium.Domain.Enums;

/// <summary>Состояние платежной операции.</summary>
public enum PaymentStatus
{
    Pending,
    Paid,
    Refunded,
    Failed
}