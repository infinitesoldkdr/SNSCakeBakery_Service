using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Controllers;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.DTOs;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNSCakeBakery_Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDto dto, string userId)
        {
            var order = new Order
            {
                UserId = userId,
                CakeType = dto.CakeType,
                Size = dto.Size,
                Message = dto.Message,
                OrderDate = DateTime.UtcNow
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _db.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id, string userId)
        {
            return await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }
    }
}
