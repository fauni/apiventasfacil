using Core.DTOs.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IWarehouseService
    {
        Task<List<WarehouseDto>> GetAllWarehousesAsync();
        Task<WarehouseResponse> SearchWarehousesAsync(WarehouseSearchRequest request);
        Task<WarehouseDto> GetWarehouseByCodeAsync(string whsCode);
    }
}
