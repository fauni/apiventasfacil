using Core.DTOs.Customer;
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
    public class PaymentGroupRepository : IPaymentGroupRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public PaymentGroupRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        public async Task<string> GetConnectionStringAsync()
        {
            var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
            return _connectionStringService.BuildSqlServerConnectionString(
                parameters.ServerDatabase,
                parameters.Database,
                parameters.UserDatabase,
                parameters.PasswordDatabase);
        }
        public async Task<List<PaymentGroupDto>> GetAllPaymentGroupsAsync()
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();
                var query = @"SELECT T0.GroupNum, T0.PymntGroup, T0.ListNum  FROM OCTG T0";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var paymentGroups = await connection.QueryAsync<PaymentGroupDto>(query);
                return paymentGroups.ToList();
            } catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving payment groups.", ex);
            }
        }

        public async Task<PaymentGroupDto> GetPaymentGroupByGroupNumAsync(string groupNum)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();
                var query = @"SELECT T0.GroupNum, T0.PymntGroup, T0.ListNum FROM OCTG T0 WHERE T0.GroupNum = @GroupNum";
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                var paymentGroup = await connection.QuerySingleAsync<PaymentGroupDto>(query, new { GroupNum = groupNum });
                if (paymentGroup == null)
                {
                    throw new Exception($"Payment group with GroupNum {groupNum} not found.");
                }
                return paymentGroup;
            }
            catch (Exception ex)
            {
                throw new Exception($"Payment group with GroupNum {groupNum} not found.", ex);
            }
        }
        public async Task<PaymentGroupResponse> SearchPaymentGroupAsync(PaymentGroupSearchRequest request)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();

                var whereClause = new StringBuilder("WHERE 1=1");
                var sqlParameters = new DynamicParameters();

                // Filtros de búsqueda
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    whereClause.Append("AND (ItemCode LIKE @searchTerm)");
                    sqlParameters.Add("@searchTerm", $"%{request.SearchTerm}%");
                }

                // Query principal
                var mainQuery = $@"
                    SELECT T0.GroupNum, T0.PymntGroup, T0.ListNum
                    FROM OCTG T0
                    {whereClause}
                    ORDER BY T0.PymntGroup
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY";
                
                // Query para contar total de registros
                var contryQuery = $@"
                    SELECT COUNT(*)
                    FROM OCTG
                    {whereClause}";

                // Calcular offset para paginación
                var offset = (request.PageNumber - 1) * request.PageSize;
                sqlParameters.Add("@offset", offset);
                sqlParameters.Add("@pageSize", request.PageSize);

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Ejecutar ambas queries
                var totalCountTask = connection.QuerySingleAsync<int>(contryQuery, sqlParameters);
                var paymentGroupsTask = connection.QueryAsync<PaymentGroupDto>(mainQuery, sqlParameters);

                await Task.WhenAll(totalCountTask, paymentGroupsTask);
                var totalCount = totalCountTask.Result;
                var paymentGroups = paymentGroupsTask.Result.ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                return new PaymentGroupResponse
                {
                    PaymentGroups = paymentGroups,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching Payment groups:{ex.Message}.", ex);
            }
        }
    }
}
