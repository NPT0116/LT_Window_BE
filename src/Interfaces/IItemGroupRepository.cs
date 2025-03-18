using System;
using src.Dto.ItemGroup;
using src.Models;
using src.Query;
using src.Utils;

namespace src.Interfaces;

public interface IItemGroupRepository
{
    Task<ItemGroupDto> GetByIdAsync(Guid id);
    Task<PagedResponse<IEnumerable<ItemGroupDto>>> GetAllAsync(ItemGroupQueryParameter itemGroupQueryParameter);
    Task AddAsync(ItemGroup entity);
    void Update(ItemGroup entity);
    Task DeleteItemGroupAsync(Guid itemGroupId);
    Task SaveChangesAsync();
}
