namespace Nimble.Modulith.Reporting.DTOs;

public class OrdersReportResponse
{
    public List<OrderDto> Orders { get; set; } = [];
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}