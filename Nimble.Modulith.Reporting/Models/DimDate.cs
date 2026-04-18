namespace Nimble.Modulith.Reporting.Models;

public class DimDate
{
    public int DateKey { get; set; }
    public DateTime Date { get; set; }
    
    public int Year { get; set; }
    public int Quarter { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = null!;
    public string MonthName { get; set; } = null!;

    public List<FactOrder> FactOrders { get; set; } = [];
}