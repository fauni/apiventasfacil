using Core.DTOs;
using Core.DTOs.SalesQuotation;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace BusinessLogic.Services
{
    public class SalesQuotationService : ISalesQuotationService
    {
        private readonly ISalesQuotationRepository _repository;


        public SalesQuotationService(ISalesQuotationRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> CreateSalesQuotationAsync(SalesQuotationDto quotationDto)
        {
            return await _repository.CreateSalesQuotationAsync(quotationDto);
        }

        public async Task<List<SalesQuotationView>> GetSalesQuotationsAsync()
        {
            return await _repository.GetSalesQuotationsAsync();
        }

        public async Task<List<SalesQuotationView>> GetSalesQuotationsFromServiceLayerAsync()
        {
            return await _repository.GetSalesQuotationsFromServiceLayerAsync();
        }
    }
}
