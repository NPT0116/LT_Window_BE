using System;
using src.Dto.Variant;

namespace src.Interfaces;

public interface IVariantRepository
{
    Task<List<VariantDto>> GetVariantByItemIdAsync(Guid id);
    Task<VariantDto> GetVariantByIdAsync(Guid id);
    Task<VariantDto> UpdateVariantAsync(UpdateVariantDto variantDto);
}
