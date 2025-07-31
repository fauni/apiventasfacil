using Core.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<CustomerSearchResponse> SearchCustomersAsync(CustomerSearchRequest request);
        Task<CustomerDto> GetCustomerByCodeAsync(string cardCode);
        Task<List<CustomerDto>> GetAllCustomersAsync();
    }
}
