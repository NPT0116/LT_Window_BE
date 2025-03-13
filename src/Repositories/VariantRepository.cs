using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.Variant;
using src.Exceptions.Variant;
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

         public async Task<PagedResponse<List<VariantDto>>> GetAllVariantsAsync(VariantQueryParameters parameters)
        {
            var query = _context.Variants
                .Include(v => v.Color)
                .Include(v => v.Item)
                    .ThenInclude(i => i.Manufacturer)
                .AsQueryable();

if (!string.IsNullOrEmpty(parameters.Search))
{
    var searchLower = parameters.Search.ToLower();
    query = query.Where(v => v.Item.ItemName.ToLower().StartsWith(searchLower));
}


            if (!string.IsNullOrEmpty(parameters.StorageFilter))
            {
                query = query.Where(v => v.Storage == parameters.StorageFilter);
            }

            if (!string.IsNullOrEmpty(parameters.ManufacturerFilter))
            {
                query = query.Where(v => v.Item.Manufacturer.ManufacturerName.Contains(parameters.ManufacturerFilter));
            }

            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "costprice":
                        query = parameters.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(v => v.CostPrice)
                            : query.OrderBy(v => v.CostPrice);
                        break;
                    case "sellingprice":
                        query = parameters.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(v => v.SellingPrice)
                            : query.OrderBy(v => v.SellingPrice);
                        break;
                    case "stockquantity":
                        query = parameters.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(v => v.StockQuantity)
                            : query.OrderBy(v => v.StockQuantity);
                        break;
                    case "storage":
                        query = SortingUtils.SortByStorage(query, parameters.SortDirection);
                        break;
                    default:
                        query = query.OrderBy(v => v.Storage);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(v => v.Storage);
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)parameters.PageSize);

            var variants = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var variantDtos = variants.Select(v => new VariantDto
            {
                VariantID = v.VariantID,
                ItemID = v.ItemID,
                ColorID = v.ColorID,
                Storage = v.Storage,
                CostPrice = v.CostPrice,
                SellingPrice = v.SellingPrice,
                StockQuantity = v.StockQuantity,
            }).ToList();

            Uri CreatePageUri(int page) => new Uri($"http://localhost:5142/api/Variant/GetAll?PageNumber={page}&PageSize={parameters.PageSize}");

            var pagedResponse = new PagedResponse<List<VariantDto>>(variantDtos, parameters.PageNumber, parameters.PageSize)
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                FirstPage = CreatePageUri(1),
                LastPage = CreatePageUri(totalPages),
                NextPage = parameters.PageNumber < totalPages ? CreatePageUri(parameters.PageNumber + 1) : null,
                PreviousPage = parameters.PageNumber > 1 ? CreatePageUri(parameters.PageNumber - 1) : null
            };

            return pagedResponse;
        }
    }
}
