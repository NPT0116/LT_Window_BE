using System;
using src.Dto.Variant;
using src.Models;

namespace src.Utils.Mapper;


    public static class VariantMapper
    {
        /// <summary>
        /// Chuyển đổi một đối tượng Variant sang VariantDto.
        /// </summary>
        /// <param name="variant">Đối tượng Variant cần chuyển đổi.</param>
        /// <returns>Đối tượng VariantDto tương ứng.</returns>
        public static VariantDto ToDto(Variant variant)
        {
            if (variant == null) return null;

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

        /// <summary>
        /// Chuyển đổi danh sách các Variant sang danh sách VariantDto.
        /// </summary>
        /// <param name="variants">Danh sách Variant cần chuyển đổi.</param>
        /// <returns>Danh sách VariantDto tương ứng.</returns>
        public static List<VariantDto> ToDtoList(IEnumerable<Variant> variants)
        {
            return variants.Select(ToDto).ToList();
        }
    }

