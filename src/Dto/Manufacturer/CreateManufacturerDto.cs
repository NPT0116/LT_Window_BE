using System;

namespace src.Dto.Manufacturer;

public class CreateManufacturerDto
{
        public string ManufacturerName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
}
