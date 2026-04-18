using FastEndpoints;
using Nimble.Modulith.Reporting.Services;

namespace Nimble.Modulith.Reporting.Endpoints.Reports;

public class ProductSalesEndpoint 
    : Endpoint<DateRangeRequest, object>
{
    private readonly IReportService _service;

    public ProductSalesEndpoint(IReportService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/reports/product-sales");
        Roles("Admin");
    }

    public override async Task HandleAsync(DateRangeRequest req, CancellationToken ct)
    {
        var result = await _service.GetProductSalesReport(req.StartDate, req.EndDate);

        if (CsvFormatter.WantsCsv(HttpContext.Request, req.Format))
        {
            var csv = CsvFormatter.Format(result);
            await Send.StringAsync(csv, 200, "text/csv", ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}