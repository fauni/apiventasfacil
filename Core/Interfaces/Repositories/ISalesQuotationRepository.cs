using Core.DTOs.SalesQuotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface ISalesQuotationRepository
    {
        Task<List<SalesQuotationView>> GetSalesQuotationsAsync();
        Task<string> CreateSalesQuotationAsync(SalesQuotationDto quotationDto);
        Task<List<SalesQuotationView>> GetSalesQuotationsFromServiceLayerAsync();
    }
}
