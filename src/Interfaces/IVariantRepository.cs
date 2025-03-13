using System;
using src.Dto.Variant;

namespace src.Interfaces;

public interface IVariantRepository
{
    Task<List<VariantDto>> GetVariantByItemIdAsync(Guid id);
}
