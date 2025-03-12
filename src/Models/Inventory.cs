using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models;
public class Inventory
    {
        [Key]
        public Guid InventoryID { get; set; }

        [ForeignKey("Variant")]
        public Guid VariantID { get; set; }
        public Variant Variant { get; set; }

        public int StockQuantity { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }