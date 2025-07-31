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
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerSearchResponse> SearchCustomersAsync(CustomerSearchRequest request)
        {
            return await _customerRepository.SearchCustomersAsync(request);
        }

        public async Task<CustomerDto> GetCustomerByCodeAsync(string cardCode)
        {
            return await _customerRepository.GetCustomerByCodeAsync(cardCode);
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllCustomersAsync();
        }
    }
}
