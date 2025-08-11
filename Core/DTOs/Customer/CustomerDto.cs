using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Customer
{
    public class CustomerDto
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardFName { get; set; }
        public string CardType { get; set; }
        public int GroupCode { get; set; }
        public string Phone1 { get; set; }
        public string LicTradNum { get; set; }
        public string Currency { get; set; }
        public int SlpCode { get; set; }
        public int ListNum { get; set; }
        public int GroupNum { get; set; }
        public string PymntGroup { get; set; }
    }

    public class CustomerSearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
    }

    public class CustomerSearchResponse
    {
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
