using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Warehouse
{
    public class WarehouseDto
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
    }

    public class WarehouseSearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
    }

    public class WarehouseResponse
    {
        public List<WarehouseDto> Warehouses { get; set; } = new List<WarehouseDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
