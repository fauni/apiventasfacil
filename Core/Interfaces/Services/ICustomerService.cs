using Core.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerSearchResponse> SearchCustomersAsync(CustomerSearchRequest request);
        Task<CustomerDto> GetCustomerByCodeAsync(string cardCode);
        Task<List<CustomerDto>> GetAllCustomersAsync();
    }


}
