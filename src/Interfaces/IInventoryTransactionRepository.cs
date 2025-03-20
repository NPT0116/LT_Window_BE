using System;
using src.Dto.InventoryTransaction;
using src.Dto.Variant;
using src.Models;

namespace src.Interfaces;

public interface IInventoryTransactionRepository
{
    Task<VariantDto> CreateInBoundTransaction(Guid variantId, int quantity);    
    Task<ICollection<VariantDto>> CreateListInboundTransaction(CreateListInboundTransactionRequest list);    
    Task<ICollection<InventoryTransactionDto>> GetInventoryTransactionsByVariantId(Guid variantId);
    Task DeleteTransaction(Guid transactionId);
}
