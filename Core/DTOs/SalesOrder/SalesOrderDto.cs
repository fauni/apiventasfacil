using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SalesOrder
{
    public class SalesOrderDto
    {
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string Comments { get; set; }
        public int? Series { get; set; }
        public int SalesPersonCode { get; set; }
        public int? ContactPersonCode { get; set; }
        public int? PaymentGroupCode { get; set; }
        public string U_usrventafacil { get; set; }
        public string? U_latitud { get; set; }
        public string? U_longitud { get; set; }
        public DateTimeOffset U_fecharegistroapp { get; set; }
        public DateTimeOffset U_horaregistroapp { get; set; }

        // Campos adicionales para SAP Service Layer
        public string? CardForeignName { get; set; }
        public string? U_codigocliente { get; set; }
        public string? U_LB_RazonSocial { get; set; }
        public string? U_NIT { get; set; }
        public string? U_LB_NIT { get; set; }
        public string? DefaultWarehouseCode { get; set; }
        public string? DefaultTaxCode { get; set; } = "IVA";

        public List<SalesOrderLineDto> DocumentLines { get; set; }
    }

    public class SalesOrderLineDto
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceAfterVAT { get; set; }
        public int UoMEntry { get; set; }

        // Campos adicionales para Service Layer
        public string? TaxCode { get; set; } = "IVA";
        public decimal DiscountPercent { get; set; } = 0.0m;
        public DateTimeOffset? ShipDate { get; set; }
        public string? WarehouseCode { get; set; }
        public string? U_descitemfacil { get; set; } // Descripción personalizable del item
        public decimal? U_PrecioVenta { get; set; }
        public decimal? U_PrecioItemVenta { get; set; }
        public string? U_TFE_codUMfact { get; set; } = "80";
        public string? U_TFE_nomUMfact { get; set; } = "FRA";
    }
}
