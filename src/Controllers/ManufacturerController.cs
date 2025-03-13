using Microsoft.AspNetCore.Mvc;
using src.Dto.Manufacturer;
using src.Interfaces;
using src.Utils;
using System;
using System.Threading.Tasks;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public ManufacturerController(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        // GET: api/Manufacturer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var manufacturerDto = await _manufacturerRepository.GetByIdAsync(id);
            if (manufacturerDto == null)
                return NotFound(new Response<object>(null, "Manufacturer not found", false));
            return Ok(new Response<ManufacturerDto>(manufacturerDto));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var manufacturerDtos = await _manufacturerRepository.GetAllAsync();
            return Ok(new Response<IEnumerable<ManufacturerDto>>(manufacturerDtos));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateManufacturerDto manufacturerDto)
        {
            var createdManufacturer = await _manufacturerRepository.CreateAsync(manufacturerDto);
            return CreatedAtAction(nameof(GetById), new { id = createdManufacturer.ManufacturerID }, new Response<ManufacturerDto>(createdManufacturer));
        }
    }
}
