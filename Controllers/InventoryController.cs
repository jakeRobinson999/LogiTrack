using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers
{
    /* API controller for inventory management */
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly IMemoryCache _cache;
        private const string cacheKey = "inventory_items";

        public InventoryController(LogiTrackContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET /api/inventory
        /* Retrieves all inventory items, with caching */
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<InventoryItem>> GetAll()
        {        

            if (_cache.TryGetValue(cacheKey, out IEnumerable<InventoryItem>? cachedItems) && cachedItems != null)
            {
                return Ok(cachedItems);
            }

            var items = _context.InventoryItems.ToList();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

            _cache.Set(cacheKey, items, cacheOptions);

            return Ok(items);
        }

        // POST /api/inventory
        /* Creates a new inventory item */
        [HttpPost]
        [Authorize]
        public ActionResult<InventoryItem> CreateItem([FromBody] InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            _context.SaveChanges();
            _cache.Remove(cacheKey);

            return CreatedAtAction(
                nameof(GetAll),
                new { id = item.ItemId },
                item
            );
        }

        // DELETE /api/inventory/{id}
        /* Deletes an inventory item by ID */
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public ActionResult DeleteItem(int id)
        {
            var item = _context.InventoryItems.Find(id);
            if (item == null)
                return NotFound();

            _context.InventoryItems.Remove(item);
            _context.SaveChanges();
            _cache.Remove(cacheKey);

            return NoContent();
        }
    }
}