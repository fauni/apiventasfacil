using Core.DTOs.TfeUnitOfMeasure;
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
    public class TfeUnitOfMeasureRepository: ITfeUnitOfMeasureRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public TfeUnitOfMeasureRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        public async Task<List<TfeUnitOfMeasureDto>> GetTfeUnitsOfMeasureAsync()
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
                    SELECT 
                        Code,
                        Name
                    FROM [@TFE_UNIDADES]
                    ORDER BY Code";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var tfeUnitsOfMeasure = await connection.QueryAsync<TfeUnitOfMeasureDto>(query);

                return tfeUnitsOfMeasure.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo unidades de medida TFE: {ex.Message}", ex);
            }
        }
    }
}
