using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Dto;
using src.Dto.Invoice;
using src.Interfaces;
using src.Query;
using src.Services;
using src.Utils;

namespace src.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly HtmlInvoiceService _htmlInvoiceService;
        private readonly PdfInvoiceService _pdfInvoiceService;

        public InvoiceController(
            IInvoiceRepository invoiceRepository,
            HtmlInvoiceService htmlInvoiceService,
            PdfInvoiceService pdfInvoiceService)
        {
            _invoiceRepository = invoiceRepository;
            _htmlInvoiceService = htmlInvoiceService;
            _pdfInvoiceService = pdfInvoiceService;
        }

        /// <summary>
        /// Tạo mới một hóa đơn.
        /// </summary>
        /// <param name="invoiceDto">Dữ liệu hóa đơn cần tạo</param>
        /// <returns>Trả về hóa đơn vừa tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto invoiceDto)
        {
            if (invoiceDto == null)
            {
                return BadRequest(new Response<InvoiceDto>
                {
                    Succeeded = false,
                    Message = "Invoice data is null.",
                    Errors = new[] { "Invoice data cannot be null." }
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

        /// <summary>
        /// Lấy danh sách hóa đơn (có hỗ trợ query theo ngày, sắp xếp, lọc,...).
        /// </summary>
        /// <param name="queryInvoiceParameter">Tham số lọc, sắp xếp</param>
        /// <returns>Danh sách hóa đơn</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices([FromQuery] InvoiceQueryParameter queryInvoiceParameter)
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync(queryInvoiceParameter);



            return Ok(invoices);
        }

        /// <summary>
        /// Lấy thông tin một hóa đơn theo ID.
        /// </summary>
        /// <param name="id">Mã định danh hóa đơn</param>
        /// <returns>Hóa đơn tương ứng</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInvoiceById([FromRoute] Guid id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new Response<InvoiceDto>
                {
                    Succeeded = false,
                    Message = $"Invoice with ID {id} not found.",
                    Errors = new[] { "Invoice not found." }
                });
            }

            var response = new Response<InvoiceDto>(invoice)
            {
                Message = "Invoice retrieved successfully.",
                Succeeded = true
            };

            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách hóa đơn của một khách hàng cụ thể.
        /// </summary>
        /// <param name="customerId">Mã định danh khách hàng</param>
        /// <returns>Danh sách hóa đơn thuộc về khách hàng đó</returns>
        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetInvoicesByCustomerId([FromRoute] Guid customerId)
        {
            var invoices = await _invoiceRepository.GetInvoicesByCustomerIdAsync(customerId);

            var response = new Response<IEnumerable<InvoiceDto>>(invoices)
            {
                Message = "Invoices retrieved successfully.",
                Succeeded = true
            };

            return Ok(response);
        }

        /// <summary>
        /// Xuất hóa đơn PDF kiểu 1 (HTMLInvoiceService).
        /// </summary>
        /// <param name="id">Mã định danh hóa đơn</param>
        /// <returns>File PDF được tạo từ HTMLInvoiceService</returns>
            [HttpGet("{id}/pdf-print")]
        public async Task<IActionResult> GetInvoicePdfPrint(Guid id)
        {
            // Gọi service tạo HTML không có ảnh, sau đó convert sang PDF
            string htmlContent = await _htmlInvoiceService.GenerateInvoiceHtmlNoImageAsync(id);
            byte[] pdfBytes = _htmlInvoiceService.ConvertHtmlToPdfUsingSelectPdf(htmlContent);
            return File(pdfBytes, "application/pdf", $"Invoice_Print_{id}.pdf");
        }

        /// <summary>
        /// API cho gửi điện tử (phiên bản có ảnh)
        /// </summary>
        [HttpGet("{id}/pdf-electronic")]
        public async Task<IActionResult> GetInvoicePdfElectronic(Guid id)
        {
            byte[] pdfBytes = await _pdfInvoiceService.GenerateInvoicePdfAsync(id);
            return File(pdfBytes, "application/pdf", $"Invoice_Electronic_{id}.pdf");
        }
    }
}
