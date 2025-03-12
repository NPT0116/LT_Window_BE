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
        
        // Tham chiếu đến bảng Color thay vì lưu thông tin màu sắc trực tiếp
        [ForeignKey("Color")]
        public Guid ColorID { get; set; }
        public Color Color { get; set; }
        
        public string? Storage { get; set; }
        public float CostPrice { get; set; }
        public float SellingPrice { get; set; }
        public int StockQuantity { get; set; }
    }