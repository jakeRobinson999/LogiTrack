using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
    /* API controller for order management */
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        public OrderController(LogiTrackContext context)
        {
            _context = context;
        }

        // GET /api/order
        /* Retrieves all orders */
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Order>> GetAll()
        {
            return Ok(_context.Orders.AsNoTracking().ToList());
        }

        // GET /api/order/{id}
        /* Retrieves a specific order with its items */
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Order> GetOrderWithItems(int id)
        {
            var order = _context.Orders.AsNoTracking().Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST /api/order
        /* Creates a new order */
        [HttpPost]
        [Authorize]
        public ActionResult<Order> CreateOrder([FromBody] Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetOrderWithItems), new { id = order.OrderId }, order);
        }

        // DELETE /api/order/{id}
        /* Deletes an order by ID */
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }
    }
}