using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Dto.Color;
using src.Dto.Item;
using src.Dto.Manufacturer;
using src.Dto.Variant;
using src.Exceptions.Variant;
using src.Interfaces;
using src.Models;
using src.Query;
using src.Utils;


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

         public async Task<PagedResponse<List<GetAllVariant>>> GetAllVariantsAsync(VariantQueryParameters parameters)
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

            if (parameters.ManufacturerFilter != null)
            {
                query = query.Where(v => v.Item.Manufacturer.ManufacturerID == parameters.ManufacturerFilter);
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
                .Include(v => v.Color)
                .Include(v => v.Item)
                .Select(v => new GetAllVariant
                {
                    VariantID = v.VariantID,
                    itemDto = new ItemDto
                    {
                        ItemID = v.Item.ItemID,
                        ItemName = v.Item.ItemName,
                    },
                    colorDto = new ColorDto
                    {
                        ColorID = v.Color.ColorID,
                        Name = v.Color.Name,
                        UrlImage = v.Color.UrlImage
                        
                    },
                    Storage = v.Storage,
                    CostPrice = v.CostPrice,
                    SellingPrice = v.SellingPrice,
                    StockQuantity = v.StockQuantity
                })
                .ToListAsync();


            Uri CreatePageUri(int page) => new Uri($"http://localhost:5142/api/Variant/GetAll?PageNumber={page}&PageSize={parameters.PageSize}");

            var pagedResponse = new PagedResponse<List<GetAllVariant>>(variants, parameters.PageNumber, parameters.PageSize)
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
         public async Task DeleteVariantAsync(Guid variantId)
        {
            var variant = await _context.Variants.FindAsync(variantId);
            if (variant == null)
            {
                throw new VariantNotFound(variantId);
            }
            // Kiểm tra xem variant có liên quan đến InvoiceDetail hay InventoryTransaction không
            bool hasInvoice = await _context.InvoiceDetails.AnyAsync(id => id.VariantID == variantId);
            bool hasInventoryTransaction = await _context.InventoryTransactions.AnyAsync(it => it.VariantID == variantId);
            if (hasInvoice || hasInventoryTransaction)
            {
                throw new VariantExistsTransactionOrInvoice(variantId);
            }
            // Lưu trữ ColorID của Variant trước khi xóa
            Guid colorId = variant.ColorID;

            // Xóa Variant
            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();

            // Kiểm tra xem Color đó còn được sử dụng bởi Variant nào khác không
            bool colorInUse = await _context.Variants.AnyAsync(v => v.ColorID == colorId);
            if (!colorInUse)
            {
                var color = await _context.Colors.FindAsync(colorId);
                if (color != null)
                {
                    _context.Colors.Remove(color);
                    await _context.SaveChangesAsync();
                }
            }
        }

     public async Task<string> GetVariantNameByIdAsync(Guid id)
        {
            // Truy vấn variant kèm theo thông tin liên quan từ bảng Item và Color
            var variant = await _context.Variants
                .Include(v => v.Item)
                .Include(v => v.Color)
                .FirstOrDefaultAsync(v => v.VariantID == id);

            if (variant == null)
                throw new ArgumentException($"Không tìm thấy variant với id: {id}", nameof(id));

            // Lấy thông tin từ các đối tượng liên quan
            string itemName = variant.Item?.ItemName ?? "Unknown Item";
            string storage = string.IsNullOrEmpty(variant.Storage) ? "No Storage" : variant.Storage;
            string colorName = variant.Color?.Name ?? "Unknown Color";

            return $"{itemName} - {storage} - {colorName}";
        }
    }
}
