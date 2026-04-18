using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Reporting.Data;
using Nimble.Modulith.Reporting.DTOs;

namespace Nimble.Modulith.Reporting.Services;

public class ReportService : IReportService
{
    private readonly ReportingDbContext _db;

    public ReportService(ReportingDbContext db)
    {
        _db = db;
    }

    public async Task<OrdersReportResponse> GetOrdersReport(DateTime start, DateTime end)
    {
        var query = _db.FactOrders
            .Where(x => x.DimDate.Date >= start && x.DimDate.Date <= end);

        var orders = await query
            .GroupBy(x => x.OrderNumber)
            .Select(g => new OrderDto
            {
                OrderNumber = g.Key,
                TotalAmount = g.Sum(x => x.TotalPrice)
            })
            .ToListAsync();

        var totalRevenue = orders.Sum(x => x.TotalAmount);
        var totalOrders = orders.Count;

        return new OrdersReportResponse
        {
            Orders = orders,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            AverageOrderValue = totalOrders == 0 ? 0 : totalRevenue / totalOrders
        };
    }

    public async Task<List<ProductSalesReportResponse>> GetProductSalesReport(DateTime start, DateTime end)
    {
        return await _db.FactOrders
            .Where(x => x.DimDate.Date >= start && x.DimDate.Date <= end)
            .GroupBy(x => new { x.DimProductId, x.DimProduct.ProductName })
            .Select(g => new ProductSalesReportResponse
            {
                ProductId = g.Key.DimProductId,
                ProductName = g.Key.ProductName,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.TotalPrice),
                OrderCount = g.Select(x => x.OrderNumber).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToListAsync();
    }

    public async Task<CustomerOrdersReportResponse> GetCustomerOrdersReport(int customerId)
    {
        var query = _db.FactOrders
            .Where(x => x.DimCustomerId == customerId);

        var orders = await query
            .GroupBy(x => x.OrderNumber)
            .Select(g => new OrderDto
            {
                OrderNumber = g.Key,
                TotalAmount = g.Sum(x => x.TotalPrice)
            })
            .ToListAsync();

        var dates = await query
            .Select(x => x.DimDate.Date)
            .ToListAsync();

        return new CustomerOrdersReportResponse
        {
            Orders = orders,
            TotalSpent = orders.Sum(x => x.TotalAmount),
            FirstOrder = dates.Min(),
            LastOrder = dates.Max()
        };
    }
}