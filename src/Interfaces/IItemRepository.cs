using System;
using src.Dto.Item;
using src.Models;

namespace src.Interfaces;

public interface IItemRepository
{
    Task<ItemDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<IEnumerable<ItemDto>> GetItemByItemGroupId(Guid ItemGroupId);
    
    Task AddAsync(Item entity);
    void Update(Item entity);
    void Delete(Item entity);
    Task SaveChangesAsync();
}
