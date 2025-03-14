using System;
using src.Dto.Invoice;

namespace src.Dto;

public class InvoiceDto
{
        public Guid InvoiceID { get; set; }

        public Guid CustomerID { get; set; }

        public float TotalAmount { get; set; }
        
        public DateTime Date { get; set; }
        public List<InvoiceDetailDto> InvoiceDetails { get; set; }

}
