using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SalesOrder
{
    public class SalesOrderView
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocType { get; set; }
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
        public int GroupNum { get; set; }
        public string PymntGroup { get; set; }
        public string U_LB_NIT { get; set; }
        public string U_LB_RazonSocial { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal VatSum { get; set; }
        public decimal DocTotal { get; set; }
        public string DocCur { get; set; }
        public string Comments { get; set; }
        public string DocStatus { get; set; }
        public List<SalesOrderLineView> Lines { get; set; } = new List<SalesOrderLineView>();
    }

    public class SalesOrderLineView
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string U_descitemfacil { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceAfVAT { get; set; }
        public string Currency { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal LineTotal { get; set; }
        public decimal GTotal { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public string UomCode { get; set; }
        public string LineStatus { get; set; }
        public string U_TFE_codUMfact { get; set; }
        public string U_TFE_nomUMfact { get; set; }
    }

    public class SalesOrderSearchRequest
    {
        public string SearchTerm { get; set; } = "";
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? CardCode { get; set; }
        public int? SlpCode { get; set; }
        public string? DocStatus { get; set; }
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
    }

    public class SalesOrderSearchResponse
    {
        public List<SalesOrderView> Orders { get; set; } = new List<SalesOrderView>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
