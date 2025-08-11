using Core.DTOs.SalesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ISalesOrderService
    {
        Task<SalesOrderSearchResponse> SearchSalesOrdersAsync(SalesOrderSearchRequest request);
        Task<SalesOrderView> GetSalesOrderByIdAsync(int docEntry);
        Task<List<SalesOrderView>> GetSalesOrdersByCustomerAsync(string cardCode, int pageSize = 20, int pageNumber = 1);
        Task<List<SalesOrderView>> GetSalesOrdersBySalesPersonAsync(int slpCode, int pageSize = 20, int pageNumber = 1);
        Task<string> CreateSalesOrderAsync(SalesOrderDto orderDto);
    }
}
