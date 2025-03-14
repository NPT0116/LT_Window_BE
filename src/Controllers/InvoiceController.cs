using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using src.Dto;
using src.Dto.Invoice;
using src.Interfaces;
using src.Utils;

namespace src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        // POST: api/invoice
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto invoiceDto)
        {
            if (invoiceDto == null)
            {
                return BadRequest(new Response<InvoiceDto>
                {
                    Succeeded = false,
                    Message = "Invoice data is null.",
                    Errors = new string[] { "Invoice data cannot be null." }
                });
            }
            
            var createdInvoice = await _invoiceRepository.CreateInvoiceAsync(invoiceDto);
            var response = new Response<InvoiceDto>(createdInvoice)
            {
                Message = "Invoice created successfully.",
                Succeeded = true
            };
            return CreatedAtAction(nameof(GetInvoiceById), new { id = createdInvoice.InvoiceID }, response);
        }

        // GET: api/invoice
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            var response = new Response<IEnumerable<InvoiceDto>>(invoices)
            {
                Message = "Invoices retrieved successfully.",
                Succeeded = true
            };
            return Ok(response);
        }

        // GET: api/invoice/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(Guid id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            var response = new Response<InvoiceDto>(invoice)
            {
                Message = "Invoice retrieved successfully.",
                Succeeded = true
            };
            return Ok(response);
        }

        // GET: api/invoice/{id}/details
        [HttpGet("Customer/{customerId}")]
        public async Task<IActionResult> GetInvoicesByCustomerId(Guid customerId)
        {
            var invoices = await _invoiceRepository.GetInvoicesByCustomerIdAsync(customerId);
            var response = new Response<IEnumerable<InvoiceDto>>(invoices)
            {
                Message = "Invoices retrieved successfully.",
                Succeeded = true
            };
            return Ok(response);
        }
    }
}
