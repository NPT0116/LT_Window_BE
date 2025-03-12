using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models;

 public class Invoice
    {
        [Key]
        public Guid InvoiceID { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerID { get; set; }
        public Customer Customer { get; set; }

        public float TotalAmount { get; set; }
        
        public DateTime Date { get; set; }

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }