using FastEndpoints;
using Nimble.Modulith.Reporting.Services;

namespace Nimble.Modulith.Reporting.Endpoints.Reports;

public class CustomerOrdersRequest
{
    public int CustomerId { get; set; }
    public string? Format { get; set; } = "json";
}

public class CustomerOrdersEndpoint 
    : Endpoint<CustomerOrdersRequest, object>
{
    private readonly IReportService _service;

    public CustomerOrdersEndpoint(IReportService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/reports/customers/{CustomerId}/orders");
        Roles("Admin");
    }

    public override async Task HandleAsync(CustomerOrdersRequest req, CancellationToken ct)
    {
        var result = await _service.GetCustomerOrdersReport(req.CustomerId);

        if (CsvFormatter.WantsCsv(HttpContext.Request, req.Format))
        {
            var csv = CsvFormatter.Format(result.Orders);
            await Send.StringAsync(csv, 200, "text/csv", cancellation: ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}