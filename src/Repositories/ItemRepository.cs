using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.Item;
using src.Interfaces;
using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Item entity)
        {
            // Thêm entity vào context, commit sẽ được gọi sau qua SaveChangesAsync
            await _context.Items.AddAsync(entity);
        }

        public void Delete(Item entity)
        {
            // Xóa entity khỏi context
            _context.Items.Remove(entity);
        }

        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            // Lấy danh sách Item từ database
            var items = await _context.Items.ToListAsync();
            // Chuyển đổi sang DTO
            return items.Select(item => new ItemDto
            {
                ItemID = item.ItemID,
                ItemGroupID = item.ItemGroupID,
                ItemName = item.ItemName,
                Description = item.Description,
                Picture = item.Picture,
                ReleaseDate = item.ReleaseDate,
                ManufacturerID = item.ManufacturerID
            });
        }

        public async Task<ItemDto> GetByIdAsync(Guid id)
        {
            // Tìm item theo id
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return null;

            // Chuyển đổi sang DTO
            return new ItemDto
            {
                ItemID = item.ItemID,
                ItemGroupID = item.ItemGroupID,
                ItemName = item.ItemName,
                Description = item.Description,
                Picture = item.Picture,
                ReleaseDate = item.ReleaseDate,
                ManufacturerID = item.ManufacturerID
            };
        }

        public Task<IEnumerable<ItemDto>> GetItemByItemGroupId(Guid ItemGroupId)
        {
            // Lấy danh sách Item theo ItemGroupId
            var items = _context.Items.Where(i => i.ItemGroupID == ItemGroupId).ToList();
            // Chuyển đổi sang DTO
            return Task.FromResult(items.Select(item => new ItemDto
            {
                ItemID = item.ItemID,
                ItemGroupID = item.ItemGroupID,
                ItemName = item.ItemName,
                Description = item.Description,
                Picture = item.Picture,
                ReleaseDate = item.ReleaseDate,
                ManufacturerID = item.ManufacturerID
            }));
        }

        public async Task SaveChangesAsync()
        {
            // Commit tất cả các thay đổi của context vào database
            await _context.SaveChangesAsync();
        }

        public void Update(Item entity)
        {
            // Cập nhật entity trong context
            _context.Items.Update(entity);
        }
    }
}
