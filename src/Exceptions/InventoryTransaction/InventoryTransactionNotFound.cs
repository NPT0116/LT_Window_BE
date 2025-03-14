using System;

namespace src.Exceptions.InventoryTransaction;

public class InventoryTransactionNotFound: BaseException
{
    public InventoryTransactionNotFound(Guid transactionId) : base($"Inventory transaction with ID {transactionId} not found.", System.Net.HttpStatusCode.NotFound)
    {
    }
}
