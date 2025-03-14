using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using src.Dto;
using src.Dto.Invoice;           // Giả sử bạn có các DTO tương ứng
namespace src.Interfaces
{
    public interface IInvoiceRepository
    {
        /// <summary>
        /// Tạo mới hóa đơn cùng với các chi tiết hóa đơn.
        /// Trong quá trình tạo, tổng tiền của hóa đơn được tính toán và đối với mỗi chi tiết,
        /// một giao dịch xuất kho (outbound) sẽ được tự động tạo để cập nhật tồn kho.
        /// </summary>
        /// <param name="invoiceDto">Thông tin hóa đơn cần tạo (bao gồm thông tin khách hàng, danh sách chi tiết hóa đơn...)</param>
        /// <returns>InvoiceDto của hóa đơn vừa được tạo</returns>
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto invoiceDto);

        /// <summary>
        /// Lấy hóa đơn theo ID.
        /// </summary>
        /// <param name="invoiceId">ID của hóa đơn cần truy xuất</param>
        /// <returns>InvoiceDto nếu tồn tại, ngược lại trả về null</returns>
        Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId);

        /// <summary>
        /// Lấy danh sách tất cả các hóa đơn.
        /// </summary>
        /// <returns>Danh sách các InvoiceDto</returns>
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();

        Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerIdAsync(Guid customerId);
    }
}
