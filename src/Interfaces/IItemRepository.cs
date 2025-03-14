using System;
using src.Dto.Item;
using src.Models;

namespace src.Interfaces;

public interface IItemRepository
{
    Task<ItemDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<IEnumerable<ItemDto>> GetItemByItemGroupId(Guid ItemGroupId);
    
    Task<ItemDto> AddItemToItemGroup(Guid ItemId, Guid ItemGroupId);
    Task CreateFullItem(CreateFullItemDto entity);
    Task<ItemDto> AddAsync(ItemCreateDto entity);
    void Update(UpdateItemDto entity);
    Task DeleteItemAsync(Guid itemId);
    Task SaveChangesAsync();
}
