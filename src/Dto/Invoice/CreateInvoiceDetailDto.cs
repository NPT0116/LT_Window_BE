using System;

namespace src.Dto.Invoice;

public class CreateInvoiceDetailDto
{
        public Guid VariantID { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
}
