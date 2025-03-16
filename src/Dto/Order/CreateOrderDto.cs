using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using src.Models;

namespace src.Dto.Order
{
    public class CreateOrderDto
    {
        public Guid CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public float TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        [Required]
        public List<CreateOrderDetailDto> OrderDetailDtos { get; set; }
    }
}
