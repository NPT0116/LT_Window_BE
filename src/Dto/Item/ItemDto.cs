using System;

namespace src.Dto.Item;

public class ItemDto
{

        public Guid ItemID { get; set; }
        
        public Guid? ItemGroupID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Picture { get; set; }
        
        public DateTime ReleaseDate { get; set; }

        public Guid ManufacturerID { get; set; }

        
}

