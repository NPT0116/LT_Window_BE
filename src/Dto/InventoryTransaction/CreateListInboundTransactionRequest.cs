using System;

namespace src.Dto.InventoryTransaction;

public class CreateListInboundTransactionRequest
{
    public ICollection<CreateInboundTransactionRequest>  list { get; set; }
}
