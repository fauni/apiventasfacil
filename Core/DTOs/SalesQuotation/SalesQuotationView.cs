using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTOs.SalesQuotation
{
    public class SalesQuotationView
    {
        public int DocEntry { get; set; }
        public string DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string U_LB_RazonSocial { get; set; }
        public string U_NIT { get; set; }
        public string Comments { get; set; }
        public string SlpCode { get; set; }
        public string SlpName { get; set; }
        public decimal DocTotal { get; set; }
        public string U_VF_TiempoEntrega { get; set; }
        public string U_VF_ValidezOferta { get; set; }
        public string U_VF_FormaPago { get; set; }
        public string U_VF_TiempoEntregaName { get; set; }
        public string U_VF_ValidezOfertaName { get; set; }
        public string U_VF_FormaPagoName { get; set; }
        public List<SalesQuotationLineView> Lines { get; set; } = new ();
    }

    public class SalesQuotationLineView
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public string UomCode { get; set; }
        public decimal PriceAfVAT { get; set; }
        public decimal LineTotal { get; set; }
        public decimal GTotal { get; set; }
    }
}
