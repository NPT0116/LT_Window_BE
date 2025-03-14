using System;

namespace src.Dto.InventoryTransaction;
    public class CreateInboundTransactionRequest
    {
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
    }