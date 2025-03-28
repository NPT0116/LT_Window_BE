using System;
using src.Dto.Variant;
using src.Query;
using src.Utils;

namespace src.Interfaces;

public interface IVariantRepository
{
    Task<List<VariantDto>> GetVariantByItemIdAsync(Guid id);
    Task<GetAllVariant> GetVariantByIdAsync(Guid id);
    Task<VariantDto> UpdateVariantAsync(UpdateVariantDto variantDto);
    Task<PagedResponse<List<GetAllVariant>>> GetAllVariantsAsync(VariantQueryParameters queryParameters);
    Task DeleteVariantAsync(Guid variantId);
    Task<string> GetVariantNameByIdAsync(Guid id);

}
