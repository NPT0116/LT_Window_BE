using System;
using src.Data;
using src.Dto.Color;
using src.Interfaces;

namespace src.Repositories;

// Không được inject IColorRepository vào ColorRepository vì ColorRepository đã triển khai IColorRepository
public class ColorRepository : IColorRepository
{
private readonly ApplicationDbContext _context;

    public ColorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ColorDto> GetByIdAsync(Guid id)
    {
        var color = await _context.Colors.FindAsync(id);
        if (color == null)
            return null;
        return new ColorDto
        {
            ColorID = color.ColorID,
            Name = color.Name,
            ItemID = color.ItemID,
            UrlImage = color.UrlImage
        };
    }
}
