using System;
using src.Dto.Manufacturer;

namespace src.Interfaces;

public interface IManufacturerRepository
{
    Task<ManufacturerDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ManufacturerDto>> GetAllAsync();
}
