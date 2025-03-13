using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.Variant;
using src.Exceptions.Variant;
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

        public async Task<VariantDto> GetVariantByIdAsync(Guid id)
        {
            var variant = await _context.Variants
                .Where(v => v.VariantID == id)
                .FirstOrDefaultAsync();
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

        public async Task<VariantDto> UpdateVariantAsync(UpdateVariantDto variantDto)
        {
            var variant = await _context.Variants.FindAsync(variantDto.VariantID);
            if (variant == null)
            {
                throw new VariantNotFound(variantDto.VariantID);
            }
            if ( variantDto.CostPrice < 0 || variantDto.SellingPrice < 0)
            {
                throw new VariantBadRequestException("Cost price or selling price must be greater than 0");
            }
            variant.CostPrice = variantDto.CostPrice;
            variant.SellingPrice = variantDto.SellingPrice;
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
