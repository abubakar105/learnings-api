using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging; // added

namespace Core_Learnings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger; // added

        public UserController(IUserService userService, ILogger<UserController> logger) // logger added
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBase<UserDto>>> GetUserById(int id)
        {
            _logger.LogInformation("GetUserById called with id={UserId}", id);

            var response = await _userService.GetUserByIdAsync(id);

            if (response.Data == null)
            {
                _logger.LogWarning("GetUserById: user not found id={UserId}", id);
                return NotFound(new ResponseBase<string>(null, "User not found", HttpStatusCode.NotFound));
            }

            _logger.LogInformation("GetUserById: user found id={UserId}", id);
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ResponseBase<List<Users>>>> GetAllUsers()
        {
            _logger.LogInformation("GetAllUsers called");

            var response = await _userService.GetAllUsersAsync();
            if (response.Data == null)
            {
                _logger.LogWarning("GetAllUsers: no users found");
                return NotFound(new ResponseBase<string>(null, "Users not found", HttpStatusCode.NotFound));
            }

            _logger.LogInformation("GetAllUsers: returned {Count} users", response.Data.Count);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase<Users>>> AddUser(UserDto user)
        {
            _logger.LogInformation("AddUser called for email={Email}", user?.Email);
            var response = await _userService.AddUserAsyncIdentity(user);
            _logger.LogInformation("AddUser finished for email={Email} with success={Success}", user?.Email, response.Status);
            return Ok(response);
        }

        [HttpPost("duplicateUser")]
        public async Task<ActionResult<ResponseBase<Users>>> CheckEmail([FromBody] CheckDuplicateUser email)
        {
            _logger.LogInformation("CheckEmail called for email={Email}", email?.Email);
            var response = await _userService.CheckEmailExists(email.Email);
            _logger.LogInformation("CheckEmail result for email={Email}: exists={Exists}", email?.Email, response?.Data != null);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDto userDto)
        {
            _logger.LogInformation("UpdateUser called id={UserId}", id);

            var response = await _userService.UpdateUserAsync(id, userDto);
            if (response.Data == null)
            {
                _logger.LogWarning("UpdateUser: user not found id={UserId}", id);
                return NotFound(new ResponseBase<string>(null, "User not found", HttpStatusCode.NotFound));
            }

            _logger.LogInformation("UpdateUser: updated user id={UserId}", id);
            return Ok(response);
        }

        [HttpGet("GetAllAdmins")]
        public async Task<ActionResult<ResponseBase<List<UsersDto>>>> GetAllAdmins()
        {
            _logger.LogInformation("GetAllAdmins called");

            var response = await _userService.GetAllAdminsAsync();
            if (response.Data == null)
            {
                _logger.LogWarning("GetAllAdmins: no admins found");
                return NotFound(new ResponseBase<string>(null, "Users not found", HttpStatusCode.NotFound));
            }

            _logger.LogInformation("GetAllAdmins: returned {Count} admins", response.Data.Count);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("DeleteUser called id={UserId}", id);

            var response = await _userService.DeleteUserAsync(id);
            if (response.Data == null)
            {
                _logger.LogWarning("DeleteUser: user not found id={UserId}", id);
                return NotFound(new ResponseBase<string>(null, "User not found", HttpStatusCode.NotFound));
            }

            _logger.LogInformation("DeleteUser: deleted user id={UserId}", id);
            return Ok(new ResponseBase<string>(null, "User deleted successfully", HttpStatusCode.OK));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation("RefreshToken called");

            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("RefreshToken: refresh token not found in cookies");
                return Unauthorized(new { message = "Refresh token not found in cookies" });
            }

            var response = await _userService.RefreshTokenAsync(refreshToken);
            if (response == null)
            {
                _logger.LogWarning("RefreshToken: invalid refresh token");
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            _logger.LogInformation("RefreshToken: token refreshed, expires={Expiry}", response.Expiration);
            return Ok(new
            {
                accessToken = response.Token,
                expires = response.Expiration
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginRequest)
        {
            _logger.LogInformation("Login attempt for email={Email}", loginRequest?.Email);

            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                _logger.LogWarning("Login: bad request - missing email or password");
                return BadRequest(new { Message = "Email and password are required." });
            }

            var response = await _userService.LoginAsync(loginRequest);

            if (response == null)
            {
                _logger.LogWarning("Login failed for email={Email}", loginRequest.Email);
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            _logger.LogInformation("Login succeeded for email={Email}, expires={Expiry}", loginRequest.Email, response.Expiration);
            return Ok(new
            {
                accessToken = response.Token,
                expires = response.Expiration
            });
        }

        [HttpPost("changeForgetPassword")]
        public async Task<ResponseBase<Users>> ChangeForgetPassword([FromBody] ResetPassword model)
        {
            _logger.LogInformation("ChangeForgetPassword called for user {User}", model?.Email);
            var response = await _userService.ChangeForgetPassword(model);
            _logger.LogInformation("ChangeForgetPassword finished for user {User} success={Success}", model?.Email, response.Status);
            return response;
        }
        [HttpPost("resetPassword")]
        public async Task<ResponseBase<Users>> ResetPassword([FromBody] ChangePasswordModel model)
        {
            _logger.LogInformation("ResetPassword called for user {User}", model.Email);
            var response = await _userService.ChangePassword(model);
            _logger.LogInformation("ResetPassword finished for user {User} success={Success}", model.Email, response.Status);
            return response;
        }
        [HttpPost("forgetPassword")]
        public async Task<ResponseBase<Users>> ForgetPassword([FromBody] CheckDuplicateUser model)
        {
            _logger.LogInformation("ForgetPassword called for email={Email}", model?.Email);
            var response = await _userService.ForgetPassword(model);

            _logger.LogInformation("ForgetPassword finished for email={Email} success={Success}", model?.Email, response.Status);
            return response;
        }

    }
}
