using System;
using src.Dto.Color;
using src.Dto.Item;

namespace src.Dto.Variant;

public class GetAllVariant
{
        public Guid VariantID { get; set; }

        public ItemDto itemDto { get; set; }
        
        public ColorDto colorDto { get; set; }        
        public string? Storage { get; set; }
        public float CostPrice { get; set; }
        public float SellingPrice { get; set; }
        public int StockQuantity { get; set; }
}
