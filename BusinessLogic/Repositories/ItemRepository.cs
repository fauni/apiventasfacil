using Core.DTOs.Item;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public class ItemRepository: IItemRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public ItemRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        public async Task<ItemSearchResponse> SearchItemsAsync(ItemSearchRequest request)
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var whereClause = new StringBuilder("WHERE T0.frozenFor = 'N'");
                var sqlParameters = new DynamicParameters();

                // Filtros de búsqueda
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    whereClause.Append(" AND (T0.ItemCode LIKE @searchTerm OR T0.ItemName LIKE @searchTerm)");
                    sqlParameters.Add("@searchTerm", $"%{request.SearchTerm}%");
                }

                // Query principal con Stock calculado
                var mainQuery = $@"
                    SELECT T0.ItemCode, T0.ItemName, T0.UgpEntry, ISNULL(SUM(T1.OnHand), 0) AS Stock
                    FROM OITM T0
                    LEFT JOIN OITW T1 ON T0.ItemCode = T1.ItemCode 
                    {whereClause}
                    GROUP BY T0.ItemCode, T0.ItemName, T0.UgpEntry
                    ORDER BY T0.ItemName
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

                // Query para contar total de registros
                var countQuery = $@"
                    SELECT COUNT(DISTINCT T0.ItemCode)
                    FROM OITM T0
                    LEFT JOIN OITW T1 ON T0.ItemCode = T1.ItemCode 
                    {whereClause}";

                // Calcular offset para paginación
                var offset = (request.PageNumber - 1) * request.PageSize;
                sqlParameters.Add("@offset", offset);
                sqlParameters.Add("@pageSize", request.PageSize);

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Ejecutar ambas queries
                var totalCountTask = connection.QuerySingleAsync<int>(countQuery, sqlParameters);
                var itemsTask = connection.QueryAsync<ItemDto>(mainQuery, sqlParameters);

                await Task.WhenAll(totalCountTask, itemsTask);

                var totalCount = await totalCountTask;
                var items = (await itemsTask).ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                return new ItemSearchResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error buscando items: {ex.Message}", ex);
            }
        }

        public async Task<ItemDto> GetItemByCodeAsync(string itemCode)
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var query = @"
                    SELECT T0.ItemCode, T0.ItemName, T0.UgpEntry, ISNULL(SUM(T1.OnHand), 0) AS Stock
                    FROM OITM T0
                    LEFT JOIN OITW T1 ON T0.ItemCode = T1.ItemCode 
                    WHERE T0.ItemCode = @itemCode
                    GROUP BY T0.ItemCode, T0.ItemName, T0.UgpEntry";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var item = await connection.QuerySingleOrDefaultAsync<ItemDto>(query, new { itemCode });

                if (item == null)
                {
                    throw new Exception($"Item con código {itemCode} no encontrado");
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo item por código {itemCode}: {ex.Message}", ex);
            }
        }

        public async Task<List<ItemAutocompleteDto>> GetItemsAutocompleteAsync(string term)
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var query = @"
                    SELECT TOP 10
                        T0.ItemCode,
                        T0.ItemName,
                        T0.ItemCode + ' - ' + T0.ItemName as DisplayText,
                        T0.UgpEntry,
                        ISNULL(SUM(T1.OnHand), 0) AS Stock
                    FROM OITM T0
                    LEFT JOIN OITW T1 ON T0.ItemCode = T1.ItemCode 
                    WHERE 
                        (T0.ItemCode LIKE @term OR T0.ItemName LIKE @term)
                        AND T0.frozenFor = 'N'
                    GROUP BY T0.ItemCode, T0.ItemName, T0.UgpEntry
                    ORDER BY T0.ItemName";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var items = await connection.QueryAsync<ItemAutocompleteDto>(query, new { term = $"%{term}%" });

                return items.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en autocompletado de items: {ex.Message}", ex);
            }
        }

        public async Task<ItemWarehouseStockResponse> GetItemStockByWarehousesAsync(string itemCode)
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                // Query para obtener información del item
                var itemQuery = @"
                SELECT ItemCode, ItemName 
                FROM OITM 
                WHERE ItemCode = @itemCode";

                // Query para obtener stock por almacenes
                var stockQuery = @"
                SELECT 
                    w.WhsCode,
                    w.WhsName,
                    ISNULL(s.OnHand, 0) AS OnHand,
                    ISNULL(s.IsCommited, 0) AS IsCommited,
                    ISNULL(s.OnOrder, 0) AS OnOrder,
                    (ISNULL(s.OnHand, 0) - ISNULL(s.IsCommited, 0) + ISNULL(s.OnOrder, 0)) AS Available
                FROM OWHS w
                LEFT JOIN OITW s ON w.WhsCode = s.WhsCode AND s.ItemCode = @itemCode
                WHERE w.Locked = 'N'
                ORDER BY w.WhsCode";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Verificar que el item existe
                var item = await connection.QueryFirstOrDefaultAsync(itemQuery, new { itemCode });
                if (item == null)
                {
                    throw new Exception($"Item con código '{itemCode}' no encontrado");
                }

                // Obtener stock por almacenes
                var warehouseStocks = await connection.QueryAsync<ItemWarehouseStockDto>(stockQuery, new { itemCode });
                var warehouseStocksList = warehouseStocks.ToList();

                // Calcular totales
                var totalOnHand = warehouseStocksList.Sum(x => x.OnHand);
                var totalIsCommited = warehouseStocksList.Sum(x => x.IsCommited);
                var totalOnOrder = warehouseStocksList.Sum(x => x.OnOrder);
                var totalAvailable = warehouseStocksList.Sum(x => x.Available);

                return new ItemWarehouseStockResponse
                {
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    WarehouseStocks = warehouseStocksList,
                    TotalOnHand = totalOnHand,
                    TotalIsCommited = totalIsCommited,
                    TotalOnOrder = totalOnOrder,
                    TotalAvailable = totalAvailable
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el stock por almacenes: {ex.Message}", ex);
            }
        }
    }
}
