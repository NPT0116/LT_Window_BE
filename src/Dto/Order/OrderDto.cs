using System;
using System.Collections.Generic;
using src.Dto.Order;
using src.Models;

namespace src.Dto
{
    public class OrderDto
    {
        public Guid OrderID { get; set; }
        public Guid CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public float TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
