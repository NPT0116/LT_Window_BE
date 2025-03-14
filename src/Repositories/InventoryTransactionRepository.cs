using System;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.InventoryTransaction;
using src.Dto.Variant;
using src.Exceptions.InventoryTransaction;
using src.Exceptions.Variant;
using src.Interfaces;
using src.Models;
using src.Utils.Mapper;

namespace src.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly ApplicationDbContext _context;
    public InventoryTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<VariantDto> CreateInBoundTransaction(Guid variantId, int quantity)
    {
        var variant = await _context.Variants.FindAsync(variantId);
        if (variant == null)
            throw new VariantNotFound(variantId);
        var transaction = new InventoryTransaction
        {
            TransactionID = Guid.NewGuid(),
            VariantID = variantId,
            TransactionType = InventoryTransactionType.Inbound,
            Quantity = quantity,
            TransactionDate = DateTime.UtcNow
        };
        var newQuantity = variant.StockQuantity + quantity;
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.InventoryTransactions.Add(transaction);
            variant.StockQuantity = newQuantity;
            _context.Variants.Update(variant);
            await _context.SaveChangesAsync();
            dbTransaction.Commit();
        }
        catch (Exception)
        {
            dbTransaction.Rollback();
            throw;
        }
            return VariantMapper.ToDto(variant);
    }

public async Task DeleteTransaction(Guid transactionId)
{
    // Lấy transaction cần xóa
    var transaction = await _context.InventoryTransactions.FindAsync(transactionId);
    if (transaction == null)
        throw new InventoryTransactionNotFound(transactionId);

    // Kiểm tra giao dịch phải là Inbound; nếu là Outbound, không cho phép xóa
    if (transaction.TransactionType != InventoryTransactionType.Inbound)
        throw new InventoryTransactionCantDeleteDueToType();
    // Lấy transaction mới nhất của variant tương ứng
    var latestTransaction = await _context.InventoryTransactions
        .Where(t => t.VariantID == transaction.VariantID)
        .OrderByDescending(t => t.TransactionDate)
        .FirstOrDefaultAsync();

    // Nếu transaction cần xóa không phải là transaction mới nhất, ném exception
    if (latestTransaction == null || latestTransaction.TransactionID != transactionId)
        throw new InventoryTransactionDeleteBlockDueToExistsOtherTransactionAbove(transactionId);

    // Lấy Variant liên quan
    var variant = await _context.Variants.FindAsync(transaction.VariantID);
    if (variant == null)
        throw new VariantNotFound(transaction.VariantID);

    // Vì đây là giao dịch Inbound, việc xóa sẽ giảm số lượng tồn kho tương ứng
    variant.StockQuantity -= transaction.Quantity;

    // Sử dụng transaction của cơ sở dữ liệu để đảm bảo tính toàn vẹn
    using var dbTransaction = await _context.Database.BeginTransactionAsync();
    try
    {
        _context.InventoryTransactions.Remove(transaction);
        _context.Variants.Update(variant);
        await _context.SaveChangesAsync();
        await dbTransaction.CommitAsync();
    }
    catch (Exception)
    {
        await dbTransaction.RollbackAsync();
        throw;
    }
}


    public async Task<ICollection<InventoryTransactionDto>> GetInventoryTransactionsByVariantId(Guid variantId)
    {
        var transactions = await _context.InventoryTransactions
            .Where(t => t.VariantID == variantId)
            .Select(t => new InventoryTransactionDto
            {
                TransactionID = t.TransactionID,
                VariantID = t.VariantID,
                TransactionType = t.TransactionType,
                Quantity = t.Quantity,
                TransactionDate = t.TransactionDate
            })
            .ToListAsync();
        return transactions;
    }
}
