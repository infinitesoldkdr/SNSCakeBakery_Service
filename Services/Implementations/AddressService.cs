using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.DTOs.Address;

namespace SNSCakeBakery_Service.Services.Address
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;

        public AddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Address>> GetAddressesAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Models.Address> GetAddressByIdAsync(string id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task<Models.Address> CreateAddressAsync(AddressDto dto)
        {
            var address = new Models.Address
            {
                UserId = dto.UserId,
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country
            };

            _context.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Models.Address> UpdateAddressAsync(string id, UpdateAddressDto dto)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return null;

            address.Street = dto.Street;
            address.City = dto.City;
            address.State = dto.State;
            address.PostalCode = dto.PostalCode;
            address.Country = dto.Country;

            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAddressAsync(string id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
