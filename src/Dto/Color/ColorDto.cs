using System;

namespace src.Dto.Color;

public class ColorDto
{
        public Guid ColorID { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        // Liên kết tới Item, tức mỗi màu thuộc về 1 item cụ thể.
        public Guid ItemID { get; set; }
        
        // URL hình ảnh thể hiện màu sắc của item
        public string UrlImage { get; set; } = string.Empty;
}
