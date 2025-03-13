using System;
using src.Dto.Variant;
using src.Query;
using src.Utils;

namespace src.Interfaces;

public interface IVariantRepository
{
    Task<List<VariantDto>> GetVariantByItemIdAsync(Guid id);
    Task<VariantDto> GetVariantByIdAsync(Guid id);
    Task<VariantDto> UpdateVariantAsync(UpdateVariantDto variantDto);
 Task<PagedResponse<List<VariantDto>>> GetAllVariantsAsync(VariantQueryParameters queryParameters);
}
