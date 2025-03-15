using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using src.Dto;
using src.Dto.Invoice;
using src.Query;           // Giả sử bạn có các DTO tương ứng
namespace src.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto invoiceDto);
        Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId);
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync( InvoiceQueryParameter queryInvoiceParameter);

        Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerIdAsync(Guid customerId);
    }
}
