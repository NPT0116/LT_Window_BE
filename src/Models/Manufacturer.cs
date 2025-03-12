using System;
using System.ComponentModel.DataAnnotations;

namespace src.Models;

    public class Manufacturer
    {
        [Key]
        public Guid ManufacturerID { get; set; }
        
        [Required]
        public string ManufacturerName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }

