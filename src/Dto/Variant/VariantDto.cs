using System;

namespace src.Dto.Variant;

public class VariantDto
{
        public Guid VariantID { get; set; }

        public Guid ItemID { get; set; }
        
        public Guid ColorID { get; set; }
        
        public string? Storage { get; set; }
        public float CostPrice { get; set; }
        public float SellingPrice { get; set; }
        public int StockQuantity { get; set; }
}
