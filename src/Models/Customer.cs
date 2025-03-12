using System;
using System.ComponentModel.DataAnnotations;

namespace src.Models;

 public class Customer
    {
        [Key]
        public Guid CustomerID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }
        
        public string? Phone { get; set; }
        
        public string? Address { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }