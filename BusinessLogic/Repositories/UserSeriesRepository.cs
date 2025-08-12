using BusinessLogic.Data;
using Core.DTOs;
using Core.DTOs.Sap;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public class UserSeriesRepository : IUserSeriesRepository
    {
        private readonly AppDbContext _context;
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public UserSeriesRepository(
            AppDbContext context,
            IParameterService parameterService,
            IConnectionStringService connectionStringService)
        {
            _context = context;
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        private async Task<string> GetSapConnectionStringAsync()
        {
            var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
            return _connectionStringService.BuildSqlServerConnectionString(
                parameters.ServerDatabase,
                parameters.Database,
                parameters.UserDatabase,
                parameters.PasswordDatabase);
        }

        public async Task<UserSerie> AssignSeriesAsync(UserSeriesDto dto)
        {
            var userSeries = new UserSerie
            {
                IdUsuario = dto.IdUsuario,
                IdSerie = dto.IdSerie,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSeries.Add(userSeries);
            await _context.SaveChangesAsync();
            return userSeries;
        }

        public async Task<IEnumerable<UserSeriesDto>> GetSeriesByUserWithDetailsAsync(int userId)
        {
            try
            {
                // Obtener series del usuario desde la base de datos local
                var userSeries = await _context
                    .UserSeries
                    .Where(us => us.IdUsuario == userId)
                    .ToListAsync();

                if (!userSeries.Any())
                {
                    return new List<UserSeriesDto>();
                }

                // Obtener conexión a SAP
                var sapConnectionString = await GetSapConnectionStringAsync();

                var result = new List<UserSeriesDto>();

                using var connection = new SqlConnection(sapConnectionString);
                await connection.OpenAsync();

                foreach (var userSerie in userSeries)
                {
                    // Consultar información de la serie en SAP
                    var query = @"
                        SELECT 
                            T0.Series,
                            T0.SeriesName,
                            T0.ObjectCode
                        FROM NNM1 T0 
                        WHERE T0.Series = @SeriesId 
                        AND T0.ObjectCode = '17'"; // 17 = Sales Orders

                    var sapSeriesInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        query,
                        new { SeriesId = userSerie.IdSerie });

                    var userSeriesWithDetails = new UserSeriesDto
                    {
                        Id = userSerie.Id,
                        IdUsuario = userSerie.IdUsuario,
                        IdSerie = userSerie.IdSerie,
                        // Datos de SAP
                        Series = sapSeriesInfo?.Series,
                        SeriesName = sapSeriesInfo?.SeriesName
                    };

                    result.Add(userSeriesWithDetails);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo series del usuario {userId} con detalles SAP: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SapSeriesDto>> GetAvailableSapSeriesAsync(int objectCode = 17)
        {
            try
            {
                var sapConnectionString = await GetSapConnectionStringAsync();

                var query = @"
                    SELECT 
                        T0.Series,
                        T0.SeriesName,
                        T0.ObjectCode,
                        T0.Indicator,
                        T0.NextNumber,
                        T0.LastNum,
                        T0.Prefix,
                        T0.Suffix,
                        T0.Remarks,
                        T0.GroupCode,
                        T0.Locked,
                        T0.PeriodValidFrom as PeriodoValidFrom,
                        T0.PeriodValidTo as PeriodoValidTo
                    FROM NNM1 T0 
                    WHERE T0.ObjectCode = @ObjectCode
                    AND T0.Locked = 'N'
                    ORDER BY T0.Series";

                using var connection = new SqlConnection(sapConnectionString);
                await connection.OpenAsync();

                var sapSeries = await connection.QueryAsync<SapSeriesDto>(query, new { ObjectCode = objectCode });

                return sapSeries.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo series disponibles de SAP para ObjectCode {objectCode}: {ex.Message}", ex);
            }
        }
    }
}
