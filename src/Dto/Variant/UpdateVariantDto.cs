using System;

namespace src.Dto.Variant;

public class UpdateVariantDto
{

        public Guid VariantID { get; set; }
        
        public float CostPrice { get; set; }
        public float SellingPrice { get; set; }
}
