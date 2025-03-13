using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.Item;
using src.Exceptions.Item;
using src.Exceptions.ItemGroup;
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

        public async Task<ItemDto> AddAsync(ItemCreateDto entity)
        {
            var newItem = new Item {
                ItemID = Guid.NewGuid(),
                ItemGroupID = entity.ItemGroupId,
                ItemName = entity.ItemName,
                Description = entity.Description,
                Picture = entity.Picture,
                ReleaseDate = entity.ReleaseDate,
                ManufacturerID = entity.ManufacturerId
            };
            await _context.Items.AddAsync(newItem);
            return new ItemDto
            {
                ItemID = newItem.ItemID,
                ItemGroupID = newItem.ItemGroupID,
                ItemName = newItem.ItemName,
                Description = newItem.Description,
                Picture = newItem.Picture,
                ReleaseDate = newItem.ReleaseDate,
                ManufacturerID = newItem.ManufacturerID
            };
        }

        public async Task<ItemDto> AddItemToItemGroup(Guid ItemId, Guid ItemGroupId)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemID == ItemId);
            if (item == null)
                throw new ItemNotFound(ItemId);
            var itemGroup = await _context.ItemGroups.FirstOrDefaultAsync(ig => ig.ItemGroupID == ItemGroupId);
            if (itemGroup == null)
                throw new ItemGroupNotFound(ItemGroupId);
            if (item.ItemGroupID != null)
                throw new ItemAlreadyInGroup(item.ItemID, item.ItemGroupID.Value);
            item.ItemGroupID = ItemGroupId;
            return new ItemDto
            {
                ItemID = item.ItemID,
                ItemGroupID = item.ItemGroupID.Value,
                ItemName = item.ItemName,
                Description = item.Description,
                Picture = item.Picture,
                ReleaseDate = item.ReleaseDate,
                ManufacturerID = item.ManufacturerID
            };
        }

        public async Task CreateFullItem(CreateFullItemDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try {
                var newItem = new Item
                {
                    ItemID = Guid.NewGuid(),
                    ItemGroupID = dto.Item.ItemGroupId, // có thể null
                    ItemName = dto.Item.ItemName,
                    Description = dto.Item.Description,
                    Picture = dto.Item.Picture,
                    ReleaseDate = dto.Item.ReleaseDate,
                    ManufacturerID = dto.Item.ManufacturerId
                };
                _context.Items.Add(newItem);
                var colorMapping = new Dictionary<int, Guid>();
                foreach (var colorDto in dto.Colors)
                {
                    var newColorId =  Guid.NewGuid();
                    // Lưu mapping: temp id -> newColorId
                    colorMapping[colorDto.TempId] = newColorId;

                    var color = new Color
                    {
                        ColorID = newColorId,
                        Name = colorDto.Name,
                        UrlImage = colorDto.UrlImage,
                        ItemID = newItem.ItemID
                    };
                    _context.Colors.Add(color);
                }
                await _context.SaveChangesAsync();
                 foreach (var variantDto in dto.Variants)
                {
                    if (!colorMapping.TryGetValue(variantDto.ColorTempId, out Guid realColorId))
                    {
                        throw new Exception($"Không tìm thấy mapping cho Color với TempId {variantDto.ColorTempId}");
                    }

                    var variant = new Variant
                    {
                        VariantID = Guid.NewGuid(),
                        ItemID = newItem.ItemID,
                        ColorID = realColorId,
                        Storage = variantDto.Storage,
                        CostPrice = variantDto.CostPrice,
                        SellingPrice = variantDto.SellingPrice,
                        StockQuantity = variantDto.StockQuantity
                    };
                    _context.Variants.Add(variant);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

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
