using Core.DTOs.UnitOfMeasure;
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
    public class UnitOfMeasureRepository : IUnitOfMeasureRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public UnitOfMeasureRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        public async Task<List<UnitOfMeasureDto>> GetUnitOfMeasuresByItemAsync(string itemCode)
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                // Tu consulta SQL exacta
                var query = @"
                    SELECT 
                        UGP1.UomEntry,
                        OUOM.UomCode,
                        OUOM.UomName,
                        UGP1.BaseQty,
                        UGP1.AltQty,
                        CASE WHEN UGP1.BaseQty = 1 THEN 1 ELSE 0 END as IsDefault,
                        OITM.ItemCode
                    FROM 
                        OITM 
                        INNER JOIN OUGP ON OITM.UgpEntry = OUGP.UgpEntry
                        INNER JOIN UGP1 ON OUGP.UgpEntry = UGP1.UgpEntry
                        INNER JOIN OUOM ON UGP1.UomEntry = OUOM.UomEntry
                    WHERE 
                        OITM.ItemCode = @itemCode
                    ORDER BY UGP1.BaseQty";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var unitOfMeasures = await connection.QueryAsync<UnitOfMeasureDto>(query, new { itemCode });

                if (!unitOfMeasures.Any())
                {
                    throw new Exception($"No se encontraron unidades de medida para el item {itemCode}");
                }

                return unitOfMeasures.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo unidades de medida para el item {itemCode}: {ex.Message}", ex);
            }
        }
    }
}