using Core.DTOs.Warehouse;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseService(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<List<WarehouseDto>> GetAllWarehousesAsync()
        {
            return await _warehouseRepository.GetAllWarehousesAsync();
        }

        public async Task<WarehouseResponse> SearchWarehousesAsync(WarehouseSearchRequest request)
        {
            return await _warehouseRepository.SearchWarehousesAsync(request);
        }

        public async Task<WarehouseDto> GetWarehouseByCodeAsync(string whsCode)
        {
            return await _warehouseRepository.GetWarehouseByCodeAsync(whsCode);
        }
    }
}
