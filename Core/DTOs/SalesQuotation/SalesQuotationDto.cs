using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SalesQuotation
{
    public class SalesQuotationDto
    {
        public string CardCode { get; set; }
        public string DocDate { get; set; }
        public List<SalesQuotationLineDto> DocumentLines { get; set; }
    }

    public class SalesQuotationLineDto
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string WarehouseCode { get; set; }
    }
}
