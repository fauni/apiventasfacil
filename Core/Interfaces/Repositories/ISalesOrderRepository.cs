using Core.DTOs.SalesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface ISalesOrderRepository
    {
        Task<SalesOrderResponseDto> CreateSalesOrderAsync(SalesOrderDto salesOrderDto);
        Task<List<SalesOrderViewDto>> GetSalesOrdersAsync();
        Task<SalesOrderViewDto?> GetSalesOrderByIdAsync(int docEntry);
    }
}
