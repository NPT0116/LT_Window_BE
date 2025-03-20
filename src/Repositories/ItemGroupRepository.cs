using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto;
using src.Dto.ItemGroup;
using src.Exceptions.ItemGroup;
using src.Interfaces;
using src.Models;
using src.Query;
using src.Utils;
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

        public async Task<PagedResponse<IEnumerable<ItemGroupDto>>> GetAllAsync(ItemGroupQueryParameter itemGroupQueryParameter)
        {
            var itemGroups =  _context.ItemGroups
            .Skip((itemGroupQueryParameter.PageNumber - 1) * itemGroupQueryParameter.PageSize).AsQueryable();
            if (itemGroupQueryParameter.ItemGroupName != null)
            {
                itemGroups = itemGroups.Where(itemGroup => itemGroup.ItemGroupName.StartsWith(itemGroupQueryParameter.ItemGroupName));
            }
            var result = await itemGroups.ToListAsync();
            var data = result.Select(itemGroup => new ItemGroupDto
            {
                ItemGroupID = itemGroup.ItemGroupID,
                ItemGroupName = itemGroup.ItemGroupName
            });
            var totalItemGroups = await _context.ItemGroups.CountAsync();
            return new PagedResponse<IEnumerable<ItemGroupDto>>(data, itemGroupQueryParameter.PageNumber, itemGroupQueryParameter.PageSize){
                TotalRecords = totalItemGroups,
                TotalPages = (int)Math.Ceiling(totalItemGroups / (double)itemGroupQueryParameter.PageSize)
            };
        }

        public async Task AddAsync(ItemGroup entity)
        {
            await _context.ItemGroups.AddAsync(entity);
        }

        public void Update(ItemGroup entity)
        {
            _context.ItemGroups.Update(entity);
        }

   public async Task DeleteItemGroupAsync(Guid itemGroupId)
        {
            var itemGroup = await _context.ItemGroups.FindAsync(itemGroupId);
            if (itemGroup == null)

                throw new ItemGroupNotFound(itemGroupId);
            // Cập nhật tất cả các Item thuộc ItemGroup này: đặt ItemGroupID thành null
            var items = await _context.Items.Where(i => i.ItemGroupID == itemGroupId).ToListAsync();
            foreach (var item in items)
            {
                item.ItemGroupID = null;
                _context.Items.Update(item);
            }

            // Xóa ItemGroup
            _context.ItemGroups.Remove(itemGroup);

            await _context.SaveChangesAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        
    }
}
