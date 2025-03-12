using Microsoft.AspNetCore.Mvc;
using src.Dto.Color;
using src.Interfaces;
using src.Utils;
using System;
using System.Threading.Tasks;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IColorRepository _colorRepository;

        public ColorController(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        // GET: api/Color/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var colorDto = await _colorRepository.GetByIdAsync(id);
            if (colorDto == null)
                return NotFound(new Response<object>(null, "Color not found", false));
            return Ok(new Response<ColorDto>(colorDto));
        }
    }
}
