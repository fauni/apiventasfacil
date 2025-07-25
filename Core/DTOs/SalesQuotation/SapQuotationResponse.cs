using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTOs.SalesQuotation
{
    public class SapQuotationResponse
    {
        public List<SalesQuotationView> Value { get; set; }
    }

    public class SalesQuotationDetailDto
    {
        public List<SalesQuotationLineView> DocumentLines { get; set; }
    }
}
