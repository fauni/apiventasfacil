using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SalesOrder
{
    public class SalesOrderViewDto
    {
        public int DocEntry { get; set; }
        public string DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal DocTotal { get; set; }
        public string DocStatus { get; set; }
        public string SalesPersonName { get; set; }
        public string Comments { get; set; }
        public List<SalesOrderLineViewDto> Lines { get; set; } = new List<SalesOrderLineViewDto>();
    }

    public class SalesOrderLineViewDto
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        public string UomCode { get; set; }
        public string WarehouseCode { get; set; }
    }
}
