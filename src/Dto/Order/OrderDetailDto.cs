using System;

namespace src.Dto.Order
{
    public class OrderDetailDto
    {
        public Guid OrderDetailID { get; set; }
        public Guid OrderID { get; set; }
        public Guid VariantID { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}
