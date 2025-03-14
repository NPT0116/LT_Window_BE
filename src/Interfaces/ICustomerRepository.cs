using System;
using src.Dto.Customer;

namespace src.Interfaces;

public interface ICustomerRepository
{
        Task<CustomerDto> CreateQuickCustomerAsync(CreateCustomerDto customerDto);
        Task<CustomerDto> GetCustomerByIdAsync(Guid customerId);


}  
