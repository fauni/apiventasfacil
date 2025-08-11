using Core.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface IPaymentGroupRepository
    {
        Task<PaymentGroupResponse> SearchPaymentGroupAsync(PaymentGroupSearchRequest request);
        Task<PaymentGroupDto> GetPaymentGroupByGroupNumAsync(string groupNum);
        Task<List<PaymentGroupDto>> GetAllPaymentGroupsAsync();
    }
}
