using System;
using src.Models;

namespace src.Dto.InventoryTransaction;

public class InventoryTransactionDto
{   
        public Guid TransactionID { get; set; }

        public Guid VariantID { get; set; }

        public InventoryTransactionType TransactionType { get; set; }

        public int Quantity { get; set; }

        public DateTime TransactionDate { get; set; }

        public Guid? InvoiceDetailID { get; set; }

}
