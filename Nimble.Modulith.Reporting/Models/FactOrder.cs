namespace Nimble.Modulith.Reporting.Models;

public class FactOrder
{
    public string OrderNumber { get; set; } = null!;
    public int OrderItemId { get; set; }

    public int DimDateId { get; set; }
    public int DimCustomerId { get; set; }
    public int DimProductId { get; set; }

    public DimDate DimDate { get; set; } = null!;
    public DimCustomer DimCustomer { get; set; } = null!;
    public DimProduct DimProduct { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal OrderTotalAmount { get; set; }
}