using Core.DTOs.TermConditions;
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
    public class TermsConditionsRepository : ITermsConditionsRepository
    {
        private readonly IParameterService _parameterService;
        private readonly IConnectionStringService _connectionStringService;

        public TermsConditionsRepository(IParameterService parameterService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _connectionStringService = connectionStringService;
        }

        public async Task<List<PaymentMethodDto>> GetPaymentMethodsAsync()
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var query = @"SELECT Code, Name FROM [@VF_FORMA_PAGO] ORDER BY Name";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var paymentMethods = await connection.QueryAsync<PaymentMethodDto>(query);

                return paymentMethods.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo formas de pago: {ex.Message}", ex);
            }
        }

        public async Task<List<DeliveryTimeDto>> GetDeliveryTimesAsync()
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var query = @"SELECT Code, Name FROM [@VF_TIEMPO_ENTREGA] ORDER BY Name";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var deliveryTimes = await connection.QueryAsync<DeliveryTimeDto>(query);

                return deliveryTimes.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo tiempos de entrega: {ex.Message}", ex);
            }
        }

        public async Task<List<OfferValidityDto>> GetOfferValiditiesAsync()
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var query = @"SELECT Code, Name FROM [@VF_VALIDEZ_OFERTA] ORDER BY Name";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var offerValidities = await connection.QueryAsync<OfferValidityDto>(query);

                return offerValidities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo validez de ofertas: {ex.Message}", ex);
            }
        }

        public async Task<TermsConditionsDto> GetAllTermsConditionsAsync()
        {
            try
            {
                // Ejecutar las consultas de forma secuencial
                var paymentMethods = await GetPaymentMethodsAsync();
                var deliveryTimes = await GetDeliveryTimesAsync();
                var offerValidities = await GetOfferValiditiesAsync();

                return new TermsConditionsDto
                {
                    PaymentMethods = paymentMethods,
                    DeliveryTimes = deliveryTimes,
                    OfferValidities = offerValidities
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo términos y condiciones: {ex.Message}", ex);
            }
        }
    }
}
