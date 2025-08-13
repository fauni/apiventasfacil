using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Item
{
    public class ItemDto
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int UgpEntry { get; set; }
        public decimal Stock { get; set; }
    }

    public class ItemSearchRequest
    {
        public string SearchTerm { get; set; } = "";
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
    }

    public class ItemSearchResponse
    {
        public List<ItemDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ItemAutocompleteDto
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string DisplayText { get; set; }
        public int UgpEntry { get; set; }
        public decimal Stock { get; set; }
    }
}
