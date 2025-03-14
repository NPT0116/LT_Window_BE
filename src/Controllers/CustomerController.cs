using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using src.Dto.Customer;
using src.Interfaces;
using src.Utils;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            return Ok(new Response<CustomerDto>(customer));
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customerDto)
        {
            var customer = await _customerRepository.CreateQuickCustomerAsync(customerDto);
            return Ok(new Response<CustomerDto>(customer));
        }
        
    }
}
