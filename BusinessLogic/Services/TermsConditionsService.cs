using Core.DTOs.TermConditions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class TermsConditionsService : ITermsConditionsService
    {
        private readonly ITermsConditionsRepository _termsConditionsRepository;

        public TermsConditionsService(ITermsConditionsRepository termsConditionsRepository)
        {
            _termsConditionsRepository = termsConditionsRepository;
        }

        public async Task<List<PaymentMethodDto>> GetPaymentMethodsAsync()
        {
            return await _termsConditionsRepository.GetPaymentMethodsAsync();
        }

        public async Task<List<DeliveryTimeDto>> GetDeliveryTimesAsync()
        {
            return await _termsConditionsRepository.GetDeliveryTimesAsync();
        }

        public async Task<List<OfferValidityDto>> GetOfferValiditiesAsync()
        {
            return await _termsConditionsRepository.GetOfferValiditiesAsync();
        }

        public async Task<TermsConditionsDto> GetAllTermsConditionsAsync()
        {
            return await _termsConditionsRepository.GetAllTermsConditionsAsync();
        }
    }
}
