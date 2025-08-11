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

        public SalesOrderService(ISalesOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<SalesOrderSearchResponse> SearchSalesOrdersAsync(SalesOrderSearchRequest request)
        {
            // Validar parámetros de entrada
            if (request.PageSize > 50) request.PageSize = 50;
            if (request.PageSize < 1) request.PageSize = 20;
            if (request.PageNumber < 1) request.PageNumber = 1;

            return await _repository.SearchSalesOrdersAsync(request);
        }

        public async Task<SalesOrderView> GetSalesOrderByIdAsync(int docEntry)
        {
            if (docEntry <= 0)
                throw new ArgumentException("DocEntry debe ser mayor a 0", nameof(docEntry));

            return await _repository.GetSalesOrderByIdAsync(docEntry);
        }

        public async Task<List<SalesOrderView>> GetSalesOrdersByCustomerAsync(string cardCode, int pageSize = 20, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(cardCode))
                throw new ArgumentException("CardCode es requerido", nameof(cardCode));

            // Validar parámetros de paginación
            if (pageSize > 50) pageSize = 50;
            if (pageSize < 1) pageSize = 20;
            if (pageNumber < 1) pageNumber = 1;

            return await _repository.GetSalesOrdersByCustomerAsync(cardCode, pageSize, pageNumber);
        }

        public async Task<List<SalesOrderView>> GetSalesOrdersBySalesPersonAsync(int slpCode, int pageSize = 20, int pageNumber = 1)
        {
            if (slpCode <= 0)
                throw new ArgumentException("SlpCode debe ser mayor a 0", nameof(slpCode));

            // Validar parámetros de paginación
            if (pageSize > 50) pageSize = 50;
            if (pageSize < 1) pageSize = 20;
            if (pageNumber < 1) pageNumber = 1;

            return await _repository.GetSalesOrdersBySalesPersonAsync(slpCode, pageSize, pageNumber);
        }

        public async Task<string> CreateSalesOrderAsync(SalesOrderDto orderDto)
        {
            // Validaciones básicas
            if (string.IsNullOrEmpty(orderDto.CardCode))
                throw new ArgumentException("CardCode es requerido");

            if (orderDto.DocumentLines == null || !orderDto.DocumentLines.Any())
                throw new ArgumentException("Debe incluir al menos una línea en la orden");

            if (orderDto.SalesPersonCode <= 0)
                throw new ArgumentException("SalesPersonCode es requerido");

            return await _repository.CreateSalesOrderAsync(orderDto);
        }
    }
}
