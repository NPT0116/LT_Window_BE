using Microsoft.AspNetCore.Mvc;
using src.Dto.Item;
using src.Interfaces;
using src.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;

        public ItemController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        // GET: api/Item/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var itemDto = await _itemRepository.GetByIdAsync(id);
            if (itemDto == null)
                return NotFound(new Response<object>(null, "Item not found", false));
            return Ok(new Response<ItemDto>(itemDto));
        }

        // GET: api/Item
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var itemDtos = await _itemRepository.GetAllAsync();
            return Ok(new Response<IEnumerable<ItemDto>>(itemDtos));
        }
        [HttpGet("ItemGroup/{ItemGroupId}")]
        public async Task<IActionResult> GetItemByItemGroupId(Guid ItemGroupId)
        {
            var itemDtos = await _itemRepository.GetItemByItemGroupId(ItemGroupId);
            return Ok(new Response<IEnumerable<ItemDto>>(itemDtos));
        }
    }
}
