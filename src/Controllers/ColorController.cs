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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateColorDto colorDto, [FromRoute] Guid id)
        {
            var updatedColor = await _colorRepository.UpdateAsync(id, colorDto);
            return Ok(new Response<ColorDto>(updatedColor));
        }
        [HttpGet("image/{variantId}")]
        public async Task<IActionResult> GetImageByVariantId(Guid variantId)
        {
            var urlImage = await _colorRepository.GetImageByVariantIdAsync(variantId);
            return Ok(new Response<string>(urlImage));
        }
    }
}
