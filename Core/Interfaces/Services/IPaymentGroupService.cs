using Core.DTOs.Customer;

namespace Core.Interfaces.Services
{
    public interface IPaymentGroupService
    {
        Task<PaymentGroupResponse> SearchPaymentGroupAsync(PaymentGroupSearchRequest request);
        Task<PaymentGroupDto> GetPaymentGroupByGroupNumAsync(string groupNum);
        Task<List<PaymentGroupDto>> GetAllPaymentGroupsAsync();
    }
}
