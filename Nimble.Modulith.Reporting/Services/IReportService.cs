using Nimble.Modulith.Reporting.DTOs;

namespace Nimble.Modulith.Reporting.Services;

public interface IReportService
{
    Task<OrdersReportResponse> GetOrdersReport(DateTime start, DateTime end);
    Task<List<ProductSalesReportResponse>> GetProductSalesReport(DateTime start, DateTime end);
    Task<CustomerOrdersReportResponse> GetCustomerOrdersReport(int customerId);
}