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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public CustomerRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
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
                    SELECT TOP 100 T0.CardCode, T0.CardName, T0.CardFName, T0.CardType, T0.GroupCode, T0.Phone1, T0.LicTradNum, 
                    T0.Currency, T0.SlpCode, T0.ListNum, T0.GroupNum, T1.PymntGroup
                    FROM OCRD T0
                    LEFT JOIN OCTG T1 ON T0.GroupNum = T1.GroupNum
                    WHERE T0.CardType = 'C'
                    ORDER BY T0.CardName";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var customers = await connection.QueryAsync<CustomerDto>(query);
                return customers.ToList();

            } catch (Exception ex)
            {
                throw new Exception($"Error getting all customers: {ex.Message}", ex);
            }
        }

        public async Task<CustomerDto> GetCustomerByCodeAsync(string cardCode)
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
                    SELECT T0.CardCode, T0.CardName, T0.CardFName, T0.CardType, T0.GroupCode, T0.Phone1, T0.LicTradNum, 
                    T0.Currency, T0.SlpCode, T0.ListNum, T0.GroupNum, T1.PymntGroup
                    FROM OCRD T0
                    LEFT JOIN OCTG T1 ON T0.GroupNum = T1.GroupNum
                    WHERE T0.CardType = 'C' AND T0.CardCode = @cardCode";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var customer = await connection.QuerySingleOrDefaultAsync<CustomerDto>(query, new { cardCode });

                if (customer == null)
                {
                    throw new Exception($"Customer with code {cardCode} not found");
                }
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception($"Customer with code { cardCode } not found");
            }
        }

        public async Task<CustomerSearchResponse> SearchCustomersAsync(CustomerSearchRequest request)
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(parameters.ServerDatabase, parameters.Database, parameters.UserDatabase, parameters.PasswordDatabase);

                var whereClause = new StringBuilder("WHERE T0.CardType = 'C'");
                var sqlParameters = new DynamicParameters();

                // Agregamos filtros de busqueda si se proporciona
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    whereClause.Append(" AND (T0.CardCode LIKE @searchTerm OR T0.CardName LIKE @searchTerm OR T0.CardFName LIKE @searchTerm)");
                    sqlParameters.Add("@searchTerm", $"%{request.SearchTerm}%");
                }

                // Query principal para obtener los clientes
                var mainQuery = $@"
                    SELECT T0.CardCode, T0.CardName, T0.CardFName, T0.CardType, T0.GroupCode, T0.Phone1, T0.LicTradNum, 
                    T0.Currency, T0.SlpCode, T0.ListNum, T0.GroupNum, T1.PymntGroup
                    FROM OCRD T0
                    LEFT JOIN OCTG T1 ON T0.GroupNum = T1.GroupNum 
                    {whereClause}
                    ORDER BY T0.CardName
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

                // Query para contar total de registros de clientes
                var countQuery = $@"
                SELECT COUNT(*)
                FROM OCRD T0
                LEFT JOIN OCTG T1 ON T0.GroupNum = T1.GroupNum
                {whereClause}";

                // Calcular offset para paginación
                var offset = (request.PageNumber - 1) * request.PageSize;
                sqlParameters.Add("@offset", offset);
                sqlParameters.Add("@pageSize", request.PageSize);

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Ejecutar ambas queries 
                var totalCountTask = connection.QuerySingleAsync<int>(countQuery, sqlParameters);
                var customersTask = connection.QueryAsync<CustomerDto>(mainQuery, sqlParameters);

                await Task.WhenAll(totalCountTask, customersTask);

                var totalCount = await totalCountTask;
                var customers = (await customersTask).ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                return new CustomerSearchResponse
                {
                    Customers = customers,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching customers: {ex.Message}", ex);
            }
        }
    }
}
