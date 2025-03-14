using System;
using System.ComponentModel.DataAnnotations;

namespace src.Dto.Invoice;

public class CreateInvoiceDto
{
        public Guid CustomerID { get; set; }        
        public DateTime Date { get; set; }
        public float TotalAmount { get; set; }

        [Required]
        public List<CreateInvoiceDetailDto> invoiceDetailDtos { get; set; } 
}

