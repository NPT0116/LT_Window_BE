using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models
{
    public class Order
    {
        [Key]
        public Guid OrderID { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerID { get; set; }
        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }

        // Thêm thuộc tính TotalAmount để tính tổng giá trị đơn hàng
        public float TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
namespace src.Models
{
    public enum OrderStatus
    {
        Created,
        Shipped,
        Delivered
    }
}
