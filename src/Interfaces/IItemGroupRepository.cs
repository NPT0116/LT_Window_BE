using System;
using src.Dto.ItemGroup;
using src.Models;

namespace src.Interfaces;

public interface IItemGroupRepository
{
    Task<ItemGroupDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ItemGroupDto>> GetAllAsync();
    Task AddAsync(ItemGroup entity);
    void Update(ItemGroup entity);
    void Delete(ItemGroup entity);
    Task SaveChangesAsync();
}
