using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Learnings.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _addressService;

        public UserAddressController(IUserAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase<UserAddressDto>>> Create([FromBody] AddUserAddressDto dto)
        {
            var response = await _addressService.CreateAddress(dto);
            return StatusCode((int)response.Status, response);
        }

        [HttpGet]
        public async Task<ActionResult<ResponseBase<List<UserAddressDto>>>> GetAll()
        {
            var response = await _addressService.GetAllAddresses();
            return StatusCode((int)response.Status, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBase<UserAddressDto>>> GetById([FromRoute] Guid id)
        {
            var response = await _addressService.GetAddressById(id);
            return StatusCode((int)response.Status, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseBase<UserAddressDto>>> Update(
            [FromRoute] Guid id,
            [FromBody] AddUserAddressDto dto)
        {
            var response = await _addressService.UpdateAddress(id, dto);
            return StatusCode((int)response.Status, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseBase<object>>> Delete([FromRoute] Guid id)
        {
            var response = await _addressService.DeleteAddress(id);
            return StatusCode((int)response.Status, response);
        }
    }
}
