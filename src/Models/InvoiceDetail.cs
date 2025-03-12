using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models;

  public class InvoiceDetail
    {
        [Key]
        public Guid InvoiceDetailID { get; set; }

        [ForeignKey("Invoice")]
        public Guid InvoiceID { get; set; }
        public Invoice Invoice { get; set; }

        [ForeignKey("Variant")]
        public Guid VariantID { get; set; }
        public Variant Variant { get; set; }

        public int Quantity { get; set; }
        public float Price { get; set; }
    }