using Microsoft.AspNetCore.Mvc;
using src.Dto.Item;
using src.Interfaces;
using src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        [HttpPost("Create")]
        public async Task<IActionResult> CreateNewItem([FromBody] ItemCreateDto item)
        {
            var newItemResult = await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();
            return Ok(new Response<ItemDto>(new ItemDto
            {
                ItemID = newItemResult.ItemID,
                ItemGroupID = newItemResult.ItemGroupID,
                ItemName = newItemResult.ItemName,
                Description = newItemResult.Description,
                Picture = newItemResult.Picture,
                ReleaseDate = newItemResult.ReleaseDate,
                ManufacturerID = newItemResult.ManufacturerID
            }, "Item created successfully", true));
        }
        [HttpPost("{itemId}/AddToItemGroup")]
        public async Task<IActionResult> AddItemToItemGroup(Guid itemId, [FromBody] AddItemToGroupDto addItemToGroupDto)
        {
            var newItemResult = await _itemRepository.AddItemToItemGroup(itemId, addItemToGroupDto.ItemGroupId);
            await _itemRepository.SaveChangesAsync();
            return Ok(new Response<ItemDto>(newItemResult, "Item added to ItemGroup successfully", true));
        }
        [HttpPost("CreateFullItem")]
        public async Task<IActionResult> CreateFullItem([FromBody] CreateFullItemDto entity)
        {
            await _itemRepository.CreateFullItem(entity);
            await _itemRepository.SaveChangesAsync();
            return Ok(new Response<object>(null, "Full Item created successfully", true));
        }
    }
}
