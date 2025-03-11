using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models;

    public class Variant
    {
        [Key]
        public Guid VariantID { get; set; }

        [ForeignKey("Item")]
        public Guid ItemID { get; set; }
        public Item Item { get; set; }

        public string? Color { get; set; }
        public string? Storage { get; set; }
        public float CostPrice { get; set; }
        public float SellingPrice { get; set; }
        public int StockQuantity { get; set; }
    }