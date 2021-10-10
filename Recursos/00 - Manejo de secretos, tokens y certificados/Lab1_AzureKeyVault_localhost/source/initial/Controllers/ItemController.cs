using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Module4Lab5.Models;

namespace Module4Lab5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ICosmosDbService cosmosDbService;

        public ItemsController(ICosmosDbService cosmosDbService)
        {
            this.cosmosDbService = cosmosDbService;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return new ActionResult<IEnumerable<Item>>(await this.cosmosDbService.GetItemsAsync("SELECT * FROM c"));
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(long id)
        {
            var todoItem = await this.cosmosDbService.GetItemAsync(id.ToString());

            if (todoItem == null)
            {
                return this.NotFound();
            }

            return new ActionResult<Item>(todoItem);
        }

        // PUT: api/Items/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(long id, Item item)
        {
            if (id.ToString() != item.Id)
            {
                return this.BadRequest();
            }

            try
            {
                await this.cosmosDbService.UpdateItemAsync(id.ToString(), item);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.ItemExists(id.ToString()))
                {
                    return this.NotFound();
                }
                throw;
            }

            return this.NoContent();
        }

        // POST: api/Items
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async void PostItem(Item item)
        {
            item.Id = Guid.NewGuid().ToString();

            await this.cosmosDbService.AddItemAsync(item);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async void DeleteItem(long id)
        {
            await this.cosmosDbService.DeleteItemAsync(id.ToString());
        }

        private bool ItemExists(string id)
        {
            return this.cosmosDbService.GetItemsAsync("SELECT * FROM c").Result.Any(e => e.Id == id);
        }
    }
}
