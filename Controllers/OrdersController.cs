using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.Models;

namespace SNSCakeBakery_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]   // Require login for all order endpoints
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // GET: api/orders/my
        // -----------------------------
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            int userId = int.Parse(User.FindFirst("userId")!.Value);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId.ToString())
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }

        // -----------------------------
        // POST: api/orders
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            int userId = int.Parse(User.FindFirst("userId")!.Value);

            var order = new Order
            {
                UserId = userId.ToString(),
                CakeType = dto.CakeType,
                Size = dto.Size,
                Quantity = dto.Quantity,
                Notes = dto.Notes,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // -----------------------------
        // GET: api/orders/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")!.Value);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id.ToString() && o.UserId == userId.ToString());

            if (order == null) return NotFound();

            return Ok(order);
        }

        // -----------------------------
        // PUT: api/orders/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, CreateOrderDto dto)
        {
            int userId = int.Parse(User.FindFirst("userId")!.Value);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id.ToString() && o.UserId == userId.ToString());

            if (order == null) return NotFound();

            order.CakeType = dto.CakeType;
            order.Size = dto.Size;
            order.Quantity = dto.Quantity;
            order.Notes = dto.Notes;
            order.OrderDate = (DateTime)dto.OrderDate;

            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // -----------------------------
        // DELETE: api/orders/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")!.Value);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id.ToString() && o.UserId == userId.ToString());

            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order deleted." });
        }
    }

    // -----------------------------
    // DTO
    // -----------------------------
    public class CreateOrderDto
    {
        public string CakeType { get; set; } = "";
        public string Size { get; set; } = "";
        public int Quantity { get; set; }
        public string Notes { get; set; } = "";

        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}
