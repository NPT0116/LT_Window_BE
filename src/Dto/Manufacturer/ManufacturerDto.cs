using System;

namespace src.Dto.Manufacturer;

public class ManufacturerDto
{
        public Guid ManufacturerID { get; set; }
        
        public string ManufacturerName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
}
