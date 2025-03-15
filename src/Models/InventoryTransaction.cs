using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models
{
    // Enum xác định loại giao dịch
    public enum InventoryTransactionType
    {
        Inbound,   // Nhập hàng: tăng số lượng tồn
        Outbound   // Xuất hàng: giảm số lượng tồn
    }

    public class InventoryTransaction
    {
        [Key]
        public Guid TransactionID { get; set; }

        // Liên kết tới Variant để biết giao dịch thuộc về sản phẩm nào
        [ForeignKey("Variant")]
        public Guid VariantID { get; set; }
        public Variant Variant { get; set; }

        // Loại giao dịch: nhập hoặc xuất
        public InventoryTransactionType TransactionType { get; set; }

        // Số lượng giao dịch: đối với nhập, đây là số lượng thêm vào;
        // đối với xuất, đây là số lượng giảm đi.
        public int Quantity { get; set; }

        // Ngày thực hiện giao dịch
        public DateTime TransactionDate { get; set; }

        // (Tùy chọn) Nếu là giao dịch xuất hàng qua hóa đơn, có thể lưu lại ID chi tiết hóa đơn
        [ForeignKey("InvoiceDetail")]
        public Guid? InvoiceDetailID { get; set; }
        public InvoiceDetail? InvoiceDetail { get; set; }
    }
}
