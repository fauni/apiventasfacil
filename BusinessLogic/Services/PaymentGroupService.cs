using Core.DTOs.Customer;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class PaymentGroupService : IPaymentGroupService
    {
        private readonly IPaymentGroupRepository _paymentGroupRepository;

        public PaymentGroupService(IPaymentGroupRepository paymentGroupRepository)
        {
            _paymentGroupRepository = paymentGroupRepository;
        }
        public async Task<List<PaymentGroupDto>> GetAllPaymentGroupsAsync()
        {
            return await _paymentGroupRepository.GetAllPaymentGroupsAsync();
        }

        public async Task<PaymentGroupDto> GetPaymentGroupByGroupNumAsync(string groupNum)
        {
            return await _paymentGroupRepository.GetPaymentGroupByGroupNumAsync(groupNum);
        }

        public async Task<PaymentGroupResponse> SearchPaymentGroupAsync(PaymentGroupSearchRequest request)
        {
            return await _paymentGroupRepository.SearchPaymentGroupAsync(request);
        }
    }
}
