using SNSCakeBakery_Service.Controllers;
using SNSCakeBakery_Service.DTOs;
using SNSCakeBakery_Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNSCakeBakery_Service.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderDto dto, string userId);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int id, string userId);
    }
}
