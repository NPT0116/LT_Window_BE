using Microsoft.AspNetCore.Mvc;
using src.Dto.Variant;
using src.Interfaces;
using src.Utils;
using System;
using System.Threading.Tasks;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantController : ControllerBase
    {
        private readonly IVariantRepository _variantRepository;

        public VariantController(IVariantRepository variantRepository)
        {
            _variantRepository = variantRepository;
        }

        // GET: api/Variant/item/{itemId}
        [HttpGet("item/{itemId}")]
        public async Task<IActionResult> GetVariantByItemId(Guid itemId)
        {
            var variantDto = await _variantRepository.GetVariantByItemIdAsync(itemId);
            if (variantDto == null)
                return NotFound(new Response<object>(null, "Variant not found", false));
            return Ok(new Response<List<VariantDto>>(variantDto));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVariantById(Guid id)
        {
            var variantDto = await _variantRepository.GetVariantByIdAsync(id);
            if (variantDto == null)
                return NotFound(new Response<object>(null, "Variant not found", false));
            return Ok(new Response<VariantDto>(variantDto));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateVariant([FromBody] UpdateVariantDto variantDto)
        {
                var updatedVariant = await _variantRepository.UpdateVariantAsync(variantDto);
                return Ok(new Response<VariantDto>(updatedVariant));
        }
    }
}
