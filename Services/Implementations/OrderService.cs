using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;

namespace SNSCakeBakery_Service.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        // ---------------------------
        // CREATE ORDER
        // ---------------------------
        public async Task<Order?> CreateOrderAsync(string userId, Order order)
        {
            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return order;
        }

        // ---------------------------
        // GET ALL ORDERS FOR USER
        // ---------------------------
        public async Task<List<Order>> GetOrdersByUserAsync(string userId)
        {
            return await _db.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // ---------------------------
        // GET ORDER BY ID (user-protected)
        // ---------------------------
        public async Task<Order?> GetOrderByIdAsync(string id, string userId)
        {
            return await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }

        // ---------------------------
        // UPDATE ORDER
        // ---------------------------
        public async Task<Order?> UpdateOrderAsync(string id, string userId, Order updatedOrder)
        {
            var existing = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (existing == null)
                return null;

            existing.CakeType = updatedOrder.CakeType;
            existing.Size = updatedOrder.Size;
            existing.Notes = updatedOrder.Notes;
            //existing.Price = updatedOrder.Price;
            existing.DeliveryRequired = updatedOrder.DeliveryRequired;
            existing.DeliveryDate = updatedOrder.DeliveryDate;

            await _db.SaveChangesAsync();

            return existing;
        }

        // ---------------------------
        // DELETE ORDER
        // ---------------------------
        public async Task<bool> DeleteOrderAsync(string id, string userId)
        {
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return false;

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
