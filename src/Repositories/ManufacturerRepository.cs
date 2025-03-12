using Microsoft.EntityFrameworkCore;
using src.Dto.Manufacturer;
using src.Interfaces;
using src.Data;
using System;
using System.Threading.Tasks;

namespace src.Repositories
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly ApplicationDbContext _context;

        public ManufacturerRepository(ApplicationDbContext context)
        {
            _context = context;
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
