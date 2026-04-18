namespace Nimble.Modulith.Reporting.DTOs;

public class ProductSalesReportResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
}