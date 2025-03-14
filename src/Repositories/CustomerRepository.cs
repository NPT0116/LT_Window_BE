using System;
using System.Threading.Tasks;
using src.Data;
using src.Models;
using src.Dto.Customer; // Giả sử đã có các DTO tương ứng
using src.Interfaces;
using src.Exceptions.Customer;
using Microsoft.EntityFrameworkCore;    // Giả sử đã định nghĩa interface ICustomerRepository

namespace src.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Phương thức tạo nhanh khách hàng
        public async Task<CustomerDto> CreateQuickCustomerAsync(CreateCustomerDto customerDto)
        {
            if (customerDto.Email != null)
            {
            var email_check = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customerDto.Email);
            if (email_check != null)
                throw new CustomerEmailExists(customerDto.Email);
            }
            if (customerDto.Phone != null)
            {
            var phone_check = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == customerDto.Phone);
            if (phone_check != null)
                throw new CustomerPhoneExists(customerDto.Phone);
            }

            var customer = new Customer
            {
                CustomerID = Guid.NewGuid(),
                Name = customerDto.Name,
                Email = customerDto.Email,
                Phone = customerDto.Phone,
                Address = customerDto.Address
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

    public async  Task<ICollection<CustomerDto>> GetAllCustomersAsync()
        {
            return await _context.Customers.Select(c => new CustomerDto
            {
                CustomerID = c.CustomerID,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            }).ToListAsync();
        }

        public async Task<CustomerDto> GetCustomerByEmailAsync(string email)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
                throw new CustomerEmailNotFound(email);
            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

        // Phương thức truy xuất khách hàng theo ID (nếu cần)
        public async Task<CustomerDto> GetCustomerByIdAsync(Guid customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new CustomerNotFound(customerId);
            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

        public async Task<CustomerDto> GetCustomerByPhoneAsync(string phone)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);
            if (customer == null)
                throw new CustomerEmailNotFound(phone);
            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

        public async Task<CustomerDto> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto customerDto)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer == null)
                throw new CustomerNotFound(customerId);
            customer.Name = customerDto.Name;
            customer.Email = customerDto.Email;
            customer.Phone = customerDto.Phone;
            customer.Address = customerDto.Address;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }
    }
}
