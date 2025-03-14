using System;
using src.Dto.Customer;

namespace src.Interfaces;

public interface ICustomerRepository
{
        Task<ICollection<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto> CreateQuickCustomerAsync(CreateCustomerDto customerDto);
        Task<CustomerDto> GetCustomerByIdAsync(Guid customerId);
        Task<CustomerDto> GetCustomerByPhoneAsync(string phone);
        Task<CustomerDto> GetCustomerByEmailAsync(string email);
        Task<CustomerDto> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto customerDto);
        
}  
