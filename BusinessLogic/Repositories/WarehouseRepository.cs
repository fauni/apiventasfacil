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

namespace BusinessLogic.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public WarehouseRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        private async Task<string> GetConnectionStringAsync()
        {
            var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
            return _connectionStringService.BuildSqlServerConnectionString(
                parameters.ServerDatabase,
                parameters.Database,
                parameters.UserDatabase,
                parameters.PasswordDatabase);
        }

        public async Task<List<WarehouseDto>> GetAllWarehousesAsync()
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();

                var query = @"
                    SELECT WhsCode, WhsName 
                    FROM OWHS 
                    WHERE Inactive = 'N'
                    ORDER BY WhsName";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var warehouses = await connection.QueryAsync<WarehouseDto>(query);
                return warehouses.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo almacenes: {ex.Message}", ex);
            }
        }

        public async Task<WarehouseResponse> SearchWarehousesAsync(WarehouseSearchRequest request)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();

                var whereClause = new StringBuilder("WHERE Inactive = 'N'");
                var parameters = new DynamicParameters();

                // Filtros de búsqueda
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    whereClause.Append(" AND (WhsCode LIKE @SearchTerm OR WhsName LIKE @SearchTerm)");
                    parameters.Add("SearchTerm", $"%{request.SearchTerm}%");
                }

                // Consulta para obtener el total de registros
                var countQuery = $@"
                    SELECT COUNT(*) 
                    FROM OWHS 
                    {whereClause}";

                // Consulta paginada
                var query = $@"
                    SELECT WhsCode, WhsName
                    FROM OWHS
                    {whereClause}
                    ORDER BY WhsName
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var offset = (request.PageNumber - 1) * request.PageSize;
                parameters.Add("Offset", offset);
                parameters.Add("PageSize", request.PageSize);

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Obtener total de registros
                var totalCount = await connection.QuerySingleAsync<int>(countQuery, parameters);

                // Obtener datos paginados
                var warehouses = await connection.QueryAsync<WarehouseDto>(query, parameters);

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                return new WarehouseResponse
                {
                    Warehouses = warehouses.ToList(),
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error buscando almacenes: {ex.Message}", ex);
            }
        }

        public async Task<WarehouseDto> GetWarehouseByCodeAsync(string whsCode)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();

                var query = @"
                    SELECT WhsCode, WhsName 
                    FROM OWHS 
                    WHERE WhsCode = @whsCode AND Inactive = 'N'";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var warehouse = await connection.QuerySingleOrDefaultAsync<WarehouseDto>(query, new { whsCode });

                if (warehouse == null)
                {
                    throw new Exception($"Almacén con código {whsCode} no encontrado o está inactivo");
                }

                return warehouse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo almacén {whsCode}: {ex.Message}", ex);
            }
        }
    }
}
