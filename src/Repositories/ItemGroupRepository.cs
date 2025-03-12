using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto;
using src.Dto.ItemGroup;
using src.Interfaces;
using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Repositories
{
    public class ItemGroupRepository : IItemGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemGroupDto> GetByIdAsync(Guid id)
        {
            var itemGroup = await _context.ItemGroups.FindAsync(id);
            if (itemGroup == null)
                return null;

            return new ItemGroupDto
            {
                ItemGroupID = itemGroup.ItemGroupID,
                ItemGroupName = itemGroup.ItemGroupName
            };
        }

        public async Task<IEnumerable<ItemGroupDto>> GetAllAsync()
        {
            var itemGroups = await _context.ItemGroups.ToListAsync();
            return itemGroups.Select(ig => new ItemGroupDto
            {
                ItemGroupID = ig.ItemGroupID,
                ItemGroupName = ig.ItemGroupName
            });
        }

        public async Task AddAsync(ItemGroup entity)
        {
            await _context.ItemGroups.AddAsync(entity);
        }

        public void Update(ItemGroup entity)
        {
            _context.ItemGroups.Update(entity);
        }

        public void Delete(ItemGroup entity)
        {
            _context.ItemGroups.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
