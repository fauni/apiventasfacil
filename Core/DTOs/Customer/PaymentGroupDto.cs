using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Customer
{
    public class PaymentGroupDto
    {
        public int GroupNum { get; set; }
        public string PymntGroup { get; set; }
        public int ListNum { get; set; }
    }

    public class PaymentGroupSearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
    }

    public class PaymentGroupResponse
    {
        public List<PaymentGroupDto> PaymentGroups { get; set; } = new List<PaymentGroupDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
