using System;
using System.ComponentModel.DataAnnotations;

namespace src.Models;

  public class Color
    {
        [Key]
        public Guid ColorID { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        // Liên kết tới Item, tức mỗi màu thuộc về 1 item cụ thể.
        public Guid ItemID { get; set; }
        public Item Item { get; set; }
        
        // URL hình ảnh thể hiện màu sắc của item
        public string UrlImage { get; set; } = string.Empty;
    }