using Microsoft.AspNetCore.Mvc;
using src.Dto.ItemGroup;
using src.Interfaces;
using src.Models;
using src.Query;
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
        public async Task<IActionResult> GetAll([FromQuery] ItemGroupQueryParameter itemGroupQueryParameter)
        {
            var itemGroupDtos = await _itemGroupRepository.GetAllAsync(itemGroupQueryParameter);
            return Ok(new Response<IEnumerable<ItemGroupDto>>(itemGroupDtos));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ItemGroupCreateDto itemGroup)
        {
            var itemGroupCreate = new ItemGroup
            {
                ItemGroupID = Guid.NewGuid(),
                ItemGroupName = itemGroup.ItemGroupName
            };
            await _itemGroupRepository.AddAsync(itemGroupCreate);
            await _itemGroupRepository.SaveChangesAsync();
            return Ok(new Response<ItemGroupDto>(new ItemGroupDto{ ItemGroupID = itemGroupCreate.ItemGroupID, ItemGroupName=itemGroupCreate.ItemGroupName}, "ItemGroup created successfully", true));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _itemGroupRepository.DeleteItemGroupAsync(id);
            await _itemGroupRepository.SaveChangesAsync();
            return Ok(new Response<object>(null, "ItemGroup deleted successfully", true));
        }        
    }
}
