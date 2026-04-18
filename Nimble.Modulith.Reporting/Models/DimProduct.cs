namespace Nimble.Modulith.Reporting.Models;

public class DimProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;

    public ICollection<FactOrder> FactOrders { get; set; } = new List<FactOrder>();
}