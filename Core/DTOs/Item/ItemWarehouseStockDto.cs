using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Item
{
    public class ItemWarehouseStockDto
    {
        public string WhsCode { get; set; } = string.Empty;
        public string WhsName { get; set; } = string.Empty;
        public decimal OnHand { get; set; }
        public decimal IsCommited { get; set; }
        public decimal OnOrder { get; set; }
        public decimal Available { get; set; }
    }

    public class ItemWarehouseStockResponse
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public List<ItemWarehouseStockDto> WarehouseStocks { get; set; } = new List<ItemWarehouseStockDto>();
        public decimal TotalOnHand { get; set; }
        public decimal TotalIsCommited { get; set; }
        public decimal TotalOnOrder { get; set; }
        public decimal TotalAvailable { get; set; }
    }
}
