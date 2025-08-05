using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class SalesPersonService : ISalesPersonService
    {
        private readonly ISalesPersonRepository _salesPersonRepository;

        public SalesPersonService(ISalesPersonRepository salesPersonRepository)
        {
            _salesPersonRepository = salesPersonRepository;
        }

        public async Task<SalesPersonDto?> GetSalesPersonByCodeAsync(int slpCode)
        {
            if (slpCode <= 0)
            {
                throw new ArgumentException("El código del vendedor debe ser mayor a 0", nameof(slpCode));
            }

            return await _salesPersonRepository.GetSalesPersonByCodeAsync(slpCode);
        }

        public async Task<List<SalesPersonDto>> GetActiveSalesPersonsAsync()
        {
            return await _salesPersonRepository.GetActiveSalesPersonsAsync();
        }

        public async Task<bool> ValidateSalesPersonAsync(int slpCode)
        {
            if (slpCode <= 0)
            {
                return false;
            }

            return await _salesPersonRepository.ValidateSalesPersonAsync(slpCode);
        }
    }
}
