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
        public string Comments { get; set; }
        public int SalesPersonCode { get; set; }
        public string U_usrventafacil { get; set; }
        public string? U_latitud { get; set; }
        public string? U_longitud { get; set; }
        public string U_VF_TiempoEntrega { get; set; }
        public string U_VF_ValidezOferta { get; set; }
        public string U_VF_FormaPago { get; set; }
        public DateTimeOffset U_fecharegistroapp { get; set; }
        public DateTimeOffset U_horaregistroapp { get; set; }
        public List<SalesQuotationLineDto> DocumentLines { get; set; }
    }

    public class SalesQuotationLineDto
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceAfterVAT { get; set; }
        public int UoMEntry { get; set; }
        // public string UoMCode { get; set; }

    }
}
