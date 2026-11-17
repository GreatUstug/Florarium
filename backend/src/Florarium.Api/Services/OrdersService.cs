using Florarium.Api.Dtos.Cart;
using Florarium.Api.Dtos.Common;
using Florarium.Api.Dtos.Orders;
using Florarium.Api.Exceptions;
using Florarium.Data;
using Florarium.Domain.Entities;
using Florarium.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

public class OrdersService(AppDbContext db, CartService cartService)
{
    private readonly AppDbContext _db = db;

    private readonly CartService _cartService = cartService;

    public async Task<OrderCreatedResponse> CreateOrderAsync(Guid userId, CreateOrderRequest request, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var cart = await _db.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Composition)
            .ThenInclude(c => c.Items)
            .ThenInclude(i => i.Flower)
            .ThenInclude(f => f.Stock)
            .FirstOrDefaultAsync(c => c.CustomerId == userId, ct);

        if (cart == null || cart.Items.Count == 0)
            throw new ApiException(409, "cart_empty", "Корзина пуста");

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == request.AddressId && a.CustomerId == userId, ct);
        if (address == null)
            throw new ApiException(400, "validation_failed", "Адрес не найден");

        var flowerIds = cart.Items.SelectMany(i => i.Composition.Items.Select(ci => ci.FlowerId)).Distinct().ToArray();
        var stocks = await _db.Stocks
            .FromSqlInterpolated($"SELECT *, xmin FROM stock WHERE flower_id = ANY({flowerIds}) FOR UPDATE")
            .ToDictionaryAsync(s => s.FlowerId, ct);

        foreach (var cartItem in cart.Items)
        {
            if (cartItem.Composition.Items.Any(ci => !ci.Flower.IsActive))
                throw new ApiException(409, "flower_inactive", "В составе есть снятые с продажи цветы");

            foreach (var compItem in cartItem.Composition.Items)
            {
                var stock = stocks[compItem.FlowerId];
                int totalToDeduct = compItem.Qty * cartItem.Qty;

                if (stock.Available < totalToDeduct || stock.Reserved < totalToDeduct)
                    throw new ApiException(409, "out_of_stock", $"Недостаточно цветов на складе для оформления заказа");

                stock.Available -= totalToDeduct;
                stock.Reserved -= totalToDeduct;
            }
        }

        var orderNumberSql = await _db.Database
            .SqlQueryRaw<int>("SELECT nextval('order_number_seq') AS \"Value\"")
            .FirstAsync(ct);

        string orderNumber = $"ORD-{DateTime.UtcNow.Year}-{orderNumberSql:D4}";

        var order = new Order
        {
            Number = orderNumber,
            CustomerId = userId,
            Status = request.PaymentMethod == PaymentMethod.Online ? OrderStatus.New : OrderStatus.New,
            CreatedAt = DateTime.UtcNow,
            Subtotal = cart.Items.Sum(i => i.Qty * i.UnitPrice),
            Discount = 0,
            Total = cart.Items.Sum(i => i.Qty * i.UnitPrice)
        };

        foreach (var cartItem in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                CompositionId = cartItem.CompositionId,
                Qty = cartItem.Qty,
                UnitPrice = cartItem.UnitPrice
            });
        }

        order.Delivery = new Delivery
        {
            RecipientName = string.IsNullOrWhiteSpace(request.RecipientName) ? null : request.RecipientName,
            RecipientPhone = string.IsNullOrWhiteSpace(request.RecipientPhone) ? null : request.RecipientPhone,
            Street = address.Street,
            Apartment = address.Apartment,
            Entrance = address.Entrance,
            Floor = address.Floor,
            Intercom = address.Intercom,
            Date = request.Date,
            Slot = request.Slot,
            CourierNote = request.CourierNote,
            Status = DeliveryStatus.Pending
        };

        order.Payments.Add(new Payment
        {
            Method = request.PaymentMethod,
            Status = PaymentStatus.Pending,
            Amount = order.Total
        });

        _db.Orders.Add(order);

        var reservations = await _db.StockReservations.Where(r => cart.Items.Select(ci => ci.Id).Contains(r.CartItemId)).ToListAsync(ct);
        _db.StockReservations.RemoveRange(reservations);
        _db.CartItems.RemoveRange(cart.Items);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new OrderCreatedResponse
        {
            Id = order.Id,
            Number = order.Number,
            Status = order.Status.ToString().ToLower(),
            Total = order.Total,
            PaymentMethod = order.Payments.First().Method.ToString().ToLower(),
            PaymentRequired = request.PaymentMethod == PaymentMethod.Online
        };
    }

    public async Task<OrderDetailDto> PayOrderAsync(Guid userId, Guid orderId, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId, ct);

        if (order == null)
            throw new ApiException(404, "not_found", "Заказ не найден");

        if (order.Status == OrderStatus.Paid)
            throw new ApiException(409, "order_already_paid", "Заказ уже оплачен");

        var payment = order.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Pending);
        if (payment == null || payment.Method != PaymentMethod.Online)
            throw new ApiException(400, "validation_failed", "Заказ не требует онлайн-оплаты");

        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = DateTime.UtcNow;
        order.Status = OrderStatus.Paid;

        await _db.SaveChangesAsync(ct);

        return (await GetOrderByIdAsync(userId, orderId, ct))!;
    }

    public async Task<PagedResponse<OrderListDto>> GetUserOrdersAsync(Guid userId, string? statusFilter, int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Orders.AsNoTracking().Where(o => o.CustomerId == userId);

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            if (statusFilter == "active")
                query = query.Where(o => o.Status == OrderStatus.New || o.Status == OrderStatus.Paid || o.Status == OrderStatus.Assembling || o.Status == OrderStatus.Delivering);
            else if (statusFilter == "done")
                query = query.Where(o => o.Status == OrderStatus.Done);
            else if (statusFilter == "cancelled")
                query = query.Where(o => o.Status == OrderStatus.Cancelled);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                Number = o.Number,
                Status = o.Status.ToString().ToLower(),
                CreatedAt = o.CreatedAt,
                Total = o.Total,
                ItemsCount = o.Items.Count,
                PreviewPhotoUrl = o.Items.FirstOrDefault() != null ? o.Items.First().Composition.PhotoUrl : null,
                Delivery = o.Delivery != null ? new OrderDeliveryDto
                {
                    Date = o.Delivery.Date,
                    Slot = o.Delivery.Slot,
                    Status = o.Delivery.Status.ToString().ToLower()
                } : null
            })
            .ToListAsync(ct);

        return new PagedResponse<OrderListDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(Guid userId, Guid orderId, CancellationToken ct)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Composition)
            .Include(o => o.Delivery)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId, ct);

        if (order == null) return null;

        bool canCancel = order.Status == OrderStatus.New || order.Status == OrderStatus.Paid;
        bool canRepeat = order.Status == OrderStatus.Done || order.Status == OrderStatus.Cancelled;

        return new OrderDetailDto
        {
            Id = order.Id,
            Number = order.Number,
            Status = order.Status.ToString().ToLower(),
            CreatedAt = order.CreatedAt,
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Total = order.Total,
            CanCancel = canCancel,
            CanRepeat = canRepeat,
            Items = [.. order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                Qty = i.Qty,
                UnitPrice = i.UnitPrice,
                LineTotal = i.Qty * i.UnitPrice,
                CanRate = order.Status == OrderStatus.Done,
                Composition = new OrderCompositionDto
                {
                    Id = i.Composition.Id,
                    Title = i.Composition.Title,
                    PhotoUrl = i.Composition.PhotoUrl,
                    Source = i.Composition.Source.ToString().ToLower()
                }
            })],
            Delivery = order.Delivery != null ? new OrderDeliveryFullDto
            {
                RecipientName = order.Delivery.RecipientName,
                Date = order.Delivery.Date,
                Slot = order.Delivery.Slot,
                CourierNote = order.Delivery.CourierNote,
                Status = order.Delivery.Status.ToString().ToLower(),
                DeliveredAt = order.Delivery.DeliveredAt
            } : null!,
            Payments = [.. order.Payments.Select(p => new OrderPaymentDto
            {
                Id = p.Id,
                Method = p.Method.ToString().ToLower(),
                Status = p.Status.ToString().ToLower(),
                Amount = p.Amount,
                PaidAt = p.PaidAt
            })]
        };
    }

    public async Task<OrderDetailDto> CancelOrderAsync(Guid userId, Guid orderId, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var order = await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Composition).ThenInclude(c => c.Items)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId, ct);

        if (order == null)
            throw new ApiException(404, "not_found", "Заказ не найден");

        if (order.Status != OrderStatus.New && order.Status != OrderStatus.Paid)
            throw new ApiException(409, "order_not_cancellable", "Заказ уже нельзя отменить");

        var flowerIds = order.Items.SelectMany(i => i.Composition.Items.Select(ci => ci.FlowerId)).Distinct().ToArray();
        var stocks = await _db.Stocks
            .FromSqlInterpolated($"SELECT *, xmin FROM stock WHERE flower_id = ANY({flowerIds}) FOR UPDATE")
            .ToDictionaryAsync(s => s.FlowerId, ct);

        foreach (var item in order.Items)
        {
            foreach (var compItem in item.Composition.Items)
            {
                int totalToReturn = compItem.Qty * item.Qty;
                stocks[compItem.FlowerId].Available += totalToReturn;
            }
        }

        order.Status = OrderStatus.Cancelled;

        var payment = order.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Paid);
        payment?.Status = PaymentStatus.Refunded;

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return (await GetOrderByIdAsync(userId, orderId, ct))!;
    }

    public async Task<RepeatOrderResponse> RepeatOrderAsync(Guid userId, Guid orderId, CancellationToken ct)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId, ct);

        if (order == null)
            throw new ApiException(404, "not_found", "Заказ не найден");

        var response = new RepeatOrderResponse();

        foreach (var item in order.Items)
        {
            try
            {
                await _cartService.AddItemAsync(userId, new AddToCartRequest
                {
                    CompositionId = item.CompositionId,
                    Qty = item.Qty
                }, ct);
            }
            catch (ApiException ex) when (ex.Code == "out_of_stock" || ex.Code == "flower_inactive")
            {
                var comp = await _db.Compositions.FindAsync([item.CompositionId], ct);
                response.Skipped.Add(new SkippedItemDto
                {
                    CompositionId = item.CompositionId,
                    Title = comp?.Title,
                    Reason = ex.Code
                });
            }
        }

        response.Cart = await _cartService.GetCartAsync(userId, ct);
        return response;
    }
}