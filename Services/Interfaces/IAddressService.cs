using SNSCakeBakery_Service.DTOs.Address;
using SNSCakeBakery_Service.Models;

namespace SNSCakeBakery_Service.Services.Address
{
    public interface IAddressService
    {
        Task<List<Models.Address>> GetAddressesAsync(string userId);
        Task<Models.Address> GetAddressByIdAsync(string id);
        Task<Models.Address> CreateAddressAsync(AddressDto dto);
        Task<Models.Address> UpdateAddressAsync(string id, UpdateAddressDto dto);
        Task<bool> DeleteAddressAsync(string id);
    }
}
