using FastEndpoints;
using Nimble.Modulith.Reporting.Services;

namespace Nimble.Modulith.Reporting.Endpoints.Reports;

public class DateRangeRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Format { get; set; } = "json";
}

public class OrdersReportEndpoint 
    : Endpoint<DateRangeRequest, object>
{
    private readonly IReportService _service;

    public OrdersReportEndpoint(IReportService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/reports/orders");
        Roles("Admin");
    }

    public override async Task HandleAsync(DateRangeRequest req, CancellationToken ct)
    {
        var result = await _service.GetOrdersReport(req.StartDate, req.EndDate);

        if (CsvFormatter.WantsCsv(HttpContext.Request, req.Format))
        {
            var csv = CsvFormatter.Format(result.Orders);

            await Send.StringAsync(csv, 200, "text/csv", cancellation: ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}