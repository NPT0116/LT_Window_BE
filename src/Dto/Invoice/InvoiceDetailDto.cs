using System;

namespace src.Dto.Invoice;

public class InvoiceDetailDto
{
        public Guid InvoiceDetailID { get; set; }

        public Guid InvoiceID { get; set; }

        public Guid VariantID { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
}
