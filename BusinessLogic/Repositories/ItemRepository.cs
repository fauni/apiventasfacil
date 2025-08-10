using Core.DTOs.Item;
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

                var whereClause = new StringBuilder("WHERE 1=1 AND frozenFor = 'N'");
                var sqlParameters = new DynamicParameters();

                // Filtros de búsqueda
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    whereClause.Append(" AND (ItemCode LIKE @searchTerm OR ItemName LIKE @searchTerm)");
                    sqlParameters.Add("@searchTerm", $"%{request.SearchTerm}%");
                }

                // Query principal basada en tu consulta
                var mainQuery = $@"
                    SELECT ItemCode, ItemName, UgpEntry
                    FROM OITM 
                    {whereClause}
                    ORDER BY ItemName
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

                // Query para contar total de registros
                var countQuery = $@"
                    SELECT COUNT(*)
                    FROM OITM
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
                    SELECT ItemCode, ItemName, UgpEntry
                    FROM OITM 
                    WHERE ItemCode = @itemCode";

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
                        ItemCode,
                        ItemName,
                        ItemCode + ' - ' + ItemName as DisplayText,
                        UgpEntry
                    FROM OITM 
                    WHERE 
                        (ItemCode LIKE @term OR ItemName LIKE @term)
                    ORDER BY ItemName";

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
    }
}
