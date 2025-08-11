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
        public Task<PaymentGroupResponse> SearchPaymentGroupAsync(PaymentGroupSearchRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
