using Microsoft.EntityFrameworkCore;

namespace EfCoreInterview;

public class OrderService(OrdersDb db)
{
    public async Task<ICollection<OrderDto>> GetOrdersByUserId(Guid userId)
    {
        var orders = await db
            .Orders
            .Include(x => x.OrderItems)
            .ToListAsync();

        return orders
            .Where(x => x.UserId == userId)
            .Select(order => new OrderDto(
                    order.Id,
                    order.CreatedAt,
                    order.LastUpdatedAt,
                    order.OrderItems
                        .Select(item => new OrderItemDto(
                            item.ProductId,
                            item.Name,
                            item.Quantity,
                            item.UnitPrice
                        ))
                        .ToArray()
                )
            )
            .ToArray();
    }
}

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual Order Order { get; set; } = null!;
}

public class OrdersDb : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public IQueryable<OrderItem> OrderItems { get; set; }
}

public class OrderItemDto(Guid ProductId, string Name, int Quantity, decimal UnitPrice);

public class OrderDto(Guid Id, DateTime CreatedAtUtc, DateTime LastUpdatedAtUtc, ICollection<OrderItemDto> Items);