using Core.DTOs;
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
    public class SalesPersonRepository: ISalesPersonRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public SalesPersonRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
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

        public async Task<SalesPersonDto?> GetSalesPersonByCodeAsync(int slpCode)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();
                var query = @"
                    SELECT SlpCode, SlpName, Memo, Active 
                    FROM [OSLP] 
                    WHERE SlpCode = @SlpCode AND Active = 'Y'";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var salesPerson = await connection.QueryFirstOrDefaultAsync<SalesPersonDto>(
                    query,
                    new { SlpCode = slpCode });

                return salesPerson;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo vendedor con código {slpCode}: {ex.Message}", ex);
            }
        }

        public async Task<List<SalesPersonDto>> GetActiveSalesPersonsAsync()
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();
                var query = @"
                    SELECT SlpCode, SlpName, Memo, Active 
                    FROM [OSLP] 
                    WHERE Active = 'Y' 
                    ORDER BY SlpName";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var salesPersons = await connection.QueryAsync<SalesPersonDto>(query);
                return salesPersons.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo vendedores activos: {ex.Message}", ex);
            }
        }

        public async Task<bool> ValidateSalesPersonAsync(int slpCode)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();
                var query = @"
                    SELECT COUNT(1) 
                    FROM [OSLP] 
                    WHERE SlpCode = @SlpCode AND Active = 'Y'";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var count = await connection.QuerySingleAsync<int>(
                    query,
                    new { SlpCode = slpCode });

                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error validando vendedor con código {slpCode}: {ex.Message}", ex);
            }
        }
    }
}
