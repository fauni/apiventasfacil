using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TermConditions
{
    public class PaymentMethodDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class DeliveryTimeDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class OfferValidityDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class TermsConditionsDto
    {
        public List<PaymentMethodDto> PaymentMethods { get; set; }
        public List<DeliveryTimeDto> DeliveryTimes { get; set; }
        public List<OfferValidityDto> OfferValidities { get; set; }
    }
}
