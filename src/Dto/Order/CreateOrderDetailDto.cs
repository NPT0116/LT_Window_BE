using System;

namespace src.Dto.Order
{
    public class CreateOrderDetailDto
    {
        public Guid VariantID { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}
