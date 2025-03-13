using Microsoft.EntityFrameworkCore;
using src.Dto.Manufacturer;
using src.Interfaces;
using src.Data;
using System;
using System.Threading.Tasks;
using src.Models;

namespace src.Repositories
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly ApplicationDbContext _context;

        public ManufacturerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ManufacturerDto> CreateAsync(CreateManufacturerDto manufacturerDto)
        {
            var manufacturer = new Manufacturer
            {
                ManufacturerID = Guid.NewGuid(),
                ManufacturerName = manufacturerDto.ManufacturerName,
                Description = manufacturerDto.Description
            };
             _context.Manufacturers.Add(manufacturer);
            await _context.SaveChangesAsync();

            return new ManufacturerDto
            {
                ManufacturerID = manufacturer.ManufacturerID,
                ManufacturerName = manufacturer.ManufacturerName,
                Description = manufacturer.Description
            };
        }

        public async Task<IEnumerable<ManufacturerDto>> GetAllAsync()
        {
            var manufacturers = _context.Manufacturers.Select(m => new ManufacturerDto{
                ManufacturerID = m.ManufacturerID,
                ManufacturerName = m.ManufacturerName,
                Description = m.Description
            }
            );
            return await manufacturers.ToListAsync();
        }

        public async Task<ManufacturerDto> GetByIdAsync(Guid id)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer == null)
                return null;
            return new ManufacturerDto
            {
                ManufacturerID = manufacturer.ManufacturerID,
                ManufacturerName = manufacturer.ManufacturerName,
                Description = manufacturer.Description
            };
        }
    }
}
