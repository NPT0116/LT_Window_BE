using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.Variant;
using src.Interfaces;
using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Nếu bạn cập nhật interface để trả về ICollection<VariantDto>
        public async Task<List<VariantDto>> GetVariantByItemIdAsync(Guid id)
        {
            var variants = await _context.Variants
                .Where(v => v.ItemID == id)
                .ToListAsync();

            return variants.Select(v => new VariantDto
            {
                VariantID = v.VariantID,
                ItemID = v.ItemID,
                ColorID = v.ColorID,
                Storage = v.Storage,
                CostPrice = v.CostPrice,
                SellingPrice = v.SellingPrice,
                StockQuantity = v.StockQuantity
            }).ToList();
        }
    }
}
