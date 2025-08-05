using Core.DTOs.TermConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ITermsConditionsService
    {
        Task<List<PaymentMethodDto>> GetPaymentMethodsAsync();
        Task<List<DeliveryTimeDto>> GetDeliveryTimesAsync();
        Task<List<OfferValidityDto>> GetOfferValiditiesAsync();
        Task<TermsConditionsDto> GetAllTermsConditionsAsync();
    }
}
