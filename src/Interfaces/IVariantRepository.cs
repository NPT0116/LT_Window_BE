using System;
using src.Dto.Variant;

namespace src.Interfaces;

public interface IVariantRepository
{
    Task<VariantDto> GetVariantByItemIdAsync(Guid id);
}
