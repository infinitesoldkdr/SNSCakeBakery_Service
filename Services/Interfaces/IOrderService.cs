using SNSCakeBakery_Service.Models;

namespace SNSCakeBakery_Service.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string userId, Order order);
        Task<List<Order>> GetOrdersByUserAsync(string userId);
        Task<Order?> GetOrderByIdAsync(string id, string userId);
        Task<bool> DeleteOrderAsync(string id, string userId);
        Task<Order?> UpdateOrderAsync(string id, string userId, Order updatedOrder);
    }
}
