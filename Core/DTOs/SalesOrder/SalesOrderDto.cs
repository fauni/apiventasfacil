using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SalesOrder
{
    public class SalesOrderDto
    {
        public int? DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string Comments { get; set; }
        public int Series { get; set; }
        public int SalesPersonCode { get; set; }
        public int? ContactPersonCode { get; set; }
        public int PaymentGroupCode { get; set; }
        public List<SalesOrderLineDto> DocumentLines { get; set; }
        public string U_usrventafacil { get; set; }
        public string? U_latitud { get; set; }
        public string? U_longitud { get; set; }
        public DateTime U_fecharegistroapp { get; set; }
        public DateTime U_horaregistroapp { get; set; }
        public string? CardForeignName { get; set; }
        public string? U_codigocliente { get; set; }
        public string U_LB_RazonSocial { get; set; }
        public string U_NIT { get; set; }
        public string U_LB_NIT { get; set; }
    }

    public class SalesOrderLineDto
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public string TaxCode { get; set; }
        public decimal PriceAfterVAT { get; set; }
        public decimal DiscountPercent { get; set; }
        public int UoMEntry { get; set; }
        public DateTime ShipDate { get; set; }
        public string WarehouseCode { get; set; }
        public string U_descitemfacil { get; set; }
        public decimal U_PrecioVenta { get; set; }
        public decimal U_PrecioItemVenta { get; set; }
        public string U_TFE_codUMfact { get; set; }
        public string U_TFE_nomUMfact { get; set; }
    }

    public class SalesOrderResponseDto
    {
        public int DocEntry { get; set; }
        public string DocNum { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
