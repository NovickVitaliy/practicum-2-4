namespace Nimble.Modulith.Reporting.DTOs;

public class CustomerOrdersReportResponse
{
    public List<OrderDto> Orders { get; set; } = [];
    public decimal TotalSpent { get; set; }
    public DateTime FirstOrder { get; set; }
    public DateTime LastOrder { get; set; }
}