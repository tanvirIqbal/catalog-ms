using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("[controller]")]
    public class ItemsController : Controller
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IItemRepository _itemRepository;

        public ItemsController(ILogger<ItemsController> logger, IItemRepository itemRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public IEnumerable<ItemDTO> GetItems()
        {
            return _itemRepository.GetItems().Select(item => item.ConvertToDTO());
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetItem(Guid id)
        {
            var item = _itemRepository.GetItem(id);
            if (item is null)
            {
                return NotFound();
            }
            return Ok(item.ConvertToDTO());
        }

        [HttpPost]
        public ActionResult<ItemDTO> CreateItem(CreateItemDTO createItemDTO)
        {
            Item item = new Item()
            {
                Id = Guid.NewGuid(),
                Name = createItemDTO.Name,
                Price = createItemDTO.Price,
                CreatedDate = DateTime.Now
            };
            _itemRepository.Create(item);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id}, item.ConvertToDTO());
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDTO updateItemDTO)
        {
            Item existingItem = _itemRepository.GetItem(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDTO.Name;
            existingItem.Price = updateItemDTO.Price;

            _itemRepository.Update(existingItem);

            return NoContent();

        }

        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
            Item existingItem = _itemRepository.GetItem(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            _itemRepository.Delete(id);
            
            return NoContent();
        }
    }
}