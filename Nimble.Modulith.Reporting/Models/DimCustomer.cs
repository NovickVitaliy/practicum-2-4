namespace Nimble.Modulith.Reporting.Models;

public class DimCustomer
{
    public int CustomerId { get; set; }
    public string Email { get; set; } = null!;

    public ICollection<FactOrder> FactOrders { get; set; } = new List<FactOrder>();
}