using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.Variant;
using src.Interfaces;
using src.Models;
using System;
using System.Threading.Tasks;

namespace src.Repositories
{
    public class VariantRepository : IVariantRepository
    {
        private readonly ApplicationDbContext _context;

        public VariantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VariantDto> GetVariantByItemIdAsync(Guid id)
        {
            // Tìm variant theo ItemID (ở đây giả sử chỉ lấy variant đầu tiên nếu có nhiều)
            var variant = await _context.Variants.FirstOrDefaultAsync(v => v.ItemID == id);
            if (variant == null)
                return null;

            // Chuyển đổi entity Variant sang DTO VariantDto
            return new VariantDto
            {
                VariantID = variant.VariantID,
                ItemID = variant.ItemID,
                ColorID = variant.ColorID,
                Storage = variant.Storage,
                CostPrice = variant.CostPrice,
                SellingPrice = variant.SellingPrice,
                StockQuantity = variant.StockQuantity
            };
        }
    }
}
