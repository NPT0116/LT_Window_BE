using System;
using src.Dto.Color;

namespace src.Interfaces;

public interface IColorRepository
{
    Task<ColorDto> GetByIdAsync(Guid id);
}
