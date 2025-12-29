using Microsoft.AspNetCore.Mvc;
using SNSCakeBakery_Service.DTOs.Address;
using SNSCakeBakery_Service.Services.Address;

namespace SNSCakeBakery_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _service;

        public AddressController(IAddressService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(string userId)
        {
            var result = await _service.GetAddressesAsync(userId);
            return Ok(result);
        }

        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetSingle(string id)
        {
            var result = await _service.GetAddressByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAddressDto dto)
        {
            var result = await _service.CreateAddressAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateAddressDto dto)
        {
            var result = await _service.UpdateAddressAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAddressAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
