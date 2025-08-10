using Core.DTOs.SalesOrder;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _repository;
        private readonly IParameterService _parameterService;
        private readonly ISapSessionService _sapSessionService;
        private readonly ILogger<SalesOrderService> _logger;

        public SalesOrderService(
            ISalesOrderRepository repository,
            IParameterService parameterService,
            ISapSessionService sapSessionService,
            ILogger<SalesOrderService> logger)
        {
            _repository = repository;
            _parameterService = parameterService;
            _sapSessionService = sapSessionService;
            _logger = logger;
        }

        public async Task<SalesOrderResponseDto> CreateSalesOrderAsync(SalesOrderDto salesOrderDto)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de orden de venta para cliente: {CardCode}", salesOrderDto.CardCode);

                var result = await _repository.CreateSalesOrderAsync(salesOrderDto);

                _logger.LogInformation("Orden de venta creada exitosamente. DocEntry: {DocEntry}, DocNum: {DocNum}",
                    result.DocEntry, result.DocNum);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear orden de venta para cliente: {CardCode}", salesOrderDto.CardCode);
                throw new Exception($"Error al crear orden de venta: {ex.Message}", ex);
            }
        }

        public async Task<List<SalesOrderViewDto>> GetSalesOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo órdenes de venta");
                return await _repository.GetSalesOrdersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener órdenes de venta");
                throw new Exception($"Error al obtener órdenes de venta: {ex.Message}", ex);
            }
        }

        public async Task<SalesOrderViewDto?> GetSalesOrderByIdAsync(int docEntry)
        {
            try
            {
                _logger.LogInformation("Obteniendo orden de venta con DocEntry: {DocEntry}", docEntry);
                return await _repository.GetSalesOrderByIdAsync(docEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener orden de venta con DocEntry: {DocEntry}", docEntry);
                throw new Exception($"Error al obtener orden de venta: {ex.Message}", ex);
            }
        }
    }
}
