using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Core_Learnings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBase<UserDto>>> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);

            if (response.Data == null)
            {
                return NotFound(new ResponseBase<string>(null, "User not found", HttpStatusCode.NotFound));
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ResponseBase<List<Users>>>> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync();
            if(response.Data == null)
            {
                return NotFound(new ResponseBase<string>(null, "Users not found", HttpStatusCode.NotFound));
            }
            return Ok(response);
        }

        [HttpPost]
        public async  Task<ActionResult<ResponseBase<Users>>> AddUser(UserDto user)
        {
            var response = await _userService.AddUserAsyncIdentity(user);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDto userDto)
        {
            //if (id != userDto.UsrId)
            //{
            //    return BadRequest(new ResponseBase<string>(null, "User ID mismatch", HttpStatusCode.BadRequest));
            //}

            var response = await _userService.UpdateUserAsync(id,userDto);
            if (response.Data == null)
            {
                return NotFound(new ResponseBase<string>(null, "User not found", HttpStatusCode.NotFound));
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            if (response.Data == null)
            {
                return NotFound(new ResponseBase<string>(null, "User not found", HttpStatusCode.NotFound));
            }

            return Ok(new ResponseBase<string>(null,"User deleted successfully", HttpStatusCode.OK));
        }
    }
}
