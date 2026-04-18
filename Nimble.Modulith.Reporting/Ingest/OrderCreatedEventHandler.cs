using Mediator;
using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Customers.Contracts;
using Nimble.Modulith.Reporting.Data;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Ingest;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ReportingDbContext _db;

    public OrderCreatedEventHandler(ReportingDbContext db)
    {
        _db = db;
    }

    public async ValueTask Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        // ❗ 1. Idempotency check (CRITICAL)
        var exists = await _db.FactOrders
            .AnyAsync(x => x.OrderNumber == notification.OrderNumber, ct);

        if (exists)
            return;

        // ❗ 2. Upsert DimCustomer
        var customer = await _db.DimCustomers
            .FirstOrDefaultAsync(x => x.CustomerId == notification.CustomerId, ct);

        if (customer is null)
        {
            customer = new DimCustomer
            {
                CustomerId = notification.CustomerId,
                Email = notification.CustomerEmail,
            };

            _db.DimCustomers.Add(customer);
        }
        else
        {
            // Optional: update email if changed
            customer.Email = notification.CustomerEmail;
        }

        // ❗ 3. Upsert DimDate
        var date = await _db.DimDates
            .FirstOrDefaultAsync(x => x.Date == notification.OrderDate.ToDateTime(TimeOnly.MinValue), ct);

        if (date is null)
        {
            var dt = notification.OrderDate.ToDateTime(TimeOnly.MinValue);

            date = new DimDate
            {
                DateKey = ConvertToDateKey(new DateOnly(dt.Year, dt.Month, dt.Day)),
                Date = dt,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                Quarter = (dt.Month - 1) / 3 + 1,
                DayOfWeek = (int)dt.DayOfWeek,
                DayName = dt.DayOfWeek.ToString(),
                MonthName = dt.ToString("MMMM")
            };

            _db.DimDates.Add(date);
        }

        // ❗ 4. Handle products (avoid N+1 queries)
        var productIds = notification.Items.Select(i => i.ProductId).ToList();

        var existingProducts = await _db.DimProducts
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId, ct);

        var factRows = new List<FactOrder>();

        foreach (var item in notification.Items)
        {
            if (!existingProducts.TryGetValue(item.ProductId, out var product))
            {
                product = new DimProduct
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName
                };

                _db.DimProducts.Add(product);
                existingProducts[item.ProductId] = product;
            }

            // ❗ 5. Insert fact row (1 per item)
            factRows.Add(new FactOrder
            {
                OrderNumber = notification.OrderNumber,
                OrderItemId = item.Id,

                DimCustomer = customer,
                DimDate = date,
                DimProduct = product,

                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice,

                // ⚠️ You probably shouldn't store this
                OrderTotalAmount = notification.TotalAmount,
                DimCustomerId = customer.CustomerId,
                DimDateId = date.DateKey,
                DimProductId = product.ProductId,
            });
        }

        _db.FactOrders.AddRange(factRows);

        await _db.SaveChangesAsync(ct);
    }
    
    private static int ConvertToDateKey(DateOnly date)
    {
        return date.Year * 10000 + date.Month * 100 + date.Day;
    }
}