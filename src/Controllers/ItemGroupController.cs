using Microsoft.AspNetCore.Mvc;
using src.Dto.ItemGroup;
using src.Interfaces;
using src.Models;
using src.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemGroupController : ControllerBase
    {
        private readonly IItemGroupRepository _itemGroupRepository;

        public ItemGroupController(IItemGroupRepository itemGroupRepository)
        {
            _itemGroupRepository = itemGroupRepository;
        }

        // GET: api/ItemGroup/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var itemGroupDto = await _itemGroupRepository.GetByIdAsync(id);
            if (itemGroupDto == null)
                return NotFound(new Response<object>(null, "ItemGroup not found", false));
            return Ok(new Response<ItemGroupDto>(itemGroupDto));
        }

        // GET: api/ItemGroup
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var itemGroupDtos = await _itemGroupRepository.GetAllAsync();
            return Ok(new Response<IEnumerable<ItemGroupDto>>(itemGroupDtos));
        }
    }
}
