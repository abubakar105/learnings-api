using Learnings.Application.Dtos;
using Learnings.Application.Repositories.Interface;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Learnings.Infrastrcuture.Repositories.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Learnings.Infrastructure.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly LearningDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Users> _userManager;
        private readonly Dictionary<string, string> _refreshTokens = new();
        private readonly JwtSettings _jwtSettings;
        public UserService(LearningDbContext context, IUserRepository userRepository, UserManager<Users> userManager, IConfiguration configuration, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public async Task<ResponseBase<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user.Data == null)
            {
                return new ResponseBase<UserDto>(null, "User not found.", HttpStatusCode.NotFound);
            }

            var userDto = new UserDto
            {
                FirstName = user.Data.FirstName,
                LastName = user.Data.LastName,
                Email = user.Data.Email,
                PhoneNumber = user.Data.PhoneNumber
            };

            return new ResponseBase<UserDto>(userDto, "User", HttpStatusCode.OK);
        }

        public async Task<ResponseBase<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Data.Select(user => new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            }).ToList();

            return new ResponseBase<List<UserDto>>(userDtos, "Users retrieved successfully.", HttpStatusCode.OK);
        }

        public async Task<ResponseBase<UserDto>> AddUserAsync(UserDto userDto)
        {

            var user = new Users
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                PhoneNumber = userDto.PhoneNumber
            };

            var addedUser = await _userRepository.AddUserAsync(user);

            var addedUserDto = new UserDto
            {
                FirstName = addedUser.Data.FirstName,
                LastName = addedUser.Data.LastName,
                Email = addedUser.Data.Email,
                PhoneNumber = addedUser.Data.PhoneNumber
            };

            return new ResponseBase<UserDto>(addedUserDto, "User added successfully.", HttpStatusCode.Created);
        }

        public async Task<ResponseBase<UserDto>> UpdateUserAsync(int id, UserDto userDto)
        {
            var isUser = await _userRepository.GetUserByIdAsync(id);
            if (isUser.Data == null)
            {
                return new ResponseBase<UserDto>(null, "User not found.", HttpStatusCode.NotFound);
            }
            if (isUser.Data.Email == userDto.Email)
            {
                return new ResponseBase<UserDto>(userDto, "Email Already Exists.", HttpStatusCode.NotFound);
            }

            var user = new Users
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                PasswordHash = userDto.Password
            };

            await _userRepository.UpdateUserAsync(user);

            return new ResponseBase<UserDto>(userDto, "User updated successfully.", HttpStatusCode.OK);
        }

        public async Task<ResponseBase<UserDto>> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user.Data == null)
            {
                return new ResponseBase<UserDto>(null, "User not found.", HttpStatusCode.NotFound);
            }
            await _userRepository.DeleteUserAsync(id);
            var userDto = new UserDto
            {
                FirstName = user.Data.FirstName,
                LastName = user.Data.LastName,
                Email = user.Data.Email,
                PhoneNumber = user.Data.PhoneNumber
            };
            return new ResponseBase<UserDto>(userDto, "User deleted successfully.", HttpStatusCode.NoContent);
        }
        public async Task<ResponseBase<Users>> AddUserAsyncIdentity(UserDto userDto)
        {
            ResponseBase<Users> response;

            try
            {
                var existedUser = await _userManager.FindByEmailAsync(userDto.Email);
                if (existedUser != null)
                {
                    return response = new ResponseBase<Users>(null, "User already Exists.", HttpStatusCode.BadRequest);
                }
                var user = new Users
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    PasswordHash = userDto.Password
                };

                var createResult = await _userManager.CreateAsync(user, userDto.Password);

                if (createResult.Succeeded)
                {
                    var claimResult = await _userManager.AddToRoleAsync(user, "User");

                    if (claimResult.Succeeded)
                    {
                        return new ResponseBase<Users>(user, "User created successfully", HttpStatusCode.OK);
                    }
                    else
                    {
                        return new ResponseBase<Users>(null, "User created but failed to assign default role", HttpStatusCode.BadRequest)
                        {
                            Errors = claimResult.Errors.Select(e => e.Description).ToList()
                        };
                    }
                }
                else
                {
                    response = new ResponseBase<Users>(null, "Could not create user", HttpStatusCode.BadRequest)
                    {
                        Errors = createResult.Errors.Select(e => e.Description).ToList()
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while creating the user.";
                response = new ResponseBase<Users>(null, errorMessage, HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string>
            {
                ex.Message,
                ex.InnerException?.Message,
                ex.StackTrace
            }
                };
                return response;
            }
        }

        public async Task<ResponseBase<List<Users>>> GetAllUsersAsyncIdentity()
        {
            ResponseBase<List<Users>> response = null;
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                return new ResponseBase<List<Users>>(users.Data, "Users retrieved successfully.", HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while creating the user.";
                response = new ResponseBase<List<Users>>(null, errorMessage, HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string>
                            {
                                ex.Message,
                                ex.InnerException?.Message,
                                ex.StackTrace
                            }
                };
                return response;
            }

        }
        public async Task<TokenResponse> LoginAsync(LoginDto loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isPasswordValid)
                return null;

            var userRoles = await _userManager.GetRolesAsync(user);
            var tokenResponse = GenerateTokens(user, userRoles);

            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user.Id, refreshToken);
            tokenResponse.RefreshToken = refreshToken;

            return tokenResponse;
        }


        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var userId = await GetUserIdByRefreshTokenAsync(refreshToken);
            if (string.IsNullOrEmpty(userId))
                return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await IsRefreshTokenValid(userId, refreshToken))
                return null;

            var userRoles = await _userManager.GetRolesAsync(user);
            var newTokens = GenerateTokens(user, userRoles);

            var newRefreshToken = GenerateRefreshToken();
            await UpdateRefreshTokenAsync(user.Id, newRefreshToken);
            newTokens.RefreshToken = newRefreshToken;

            return newTokens;
        }

        private async Task SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            var userToken = await _userManager.GetAuthenticationTokenAsync(await _userManager.FindByIdAsync(userId), "MyApp", "RefreshToken");

            if (!string.IsNullOrEmpty(userToken))
            {
                await _userManager.RemoveAuthenticationTokenAsync(await _userManager.FindByIdAsync(userId), "MyApp", "RefreshToken");
            }

            await _userManager.SetAuthenticationTokenAsync(await _userManager.FindByIdAsync(userId), "MyApp", "RefreshToken", refreshToken);
        }
        private async Task UpdateRefreshTokenAsync(string userId, string newRefreshToken)
        {
            await SaveRefreshTokenAsync(userId, newRefreshToken);
        }
        private async Task<bool> IsRefreshTokenValid(string userId, string refreshToken)
        {
            var storedToken = await _userManager.GetAuthenticationTokenAsync(await _userManager.FindByIdAsync(userId), "MyApp", "RefreshToken");
            return storedToken == refreshToken;
        }

        public async Task<string> GetUserIdByRefreshTokenAsync(string refreshToken)
        {
            foreach (var user in _userManager.Users)
            {
                var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
                if (storedToken == refreshToken)
                {
                    return user.Id;
                }
            }
            return null;
        }


        private TokenResponse GenerateTokens(Users user, IList<string> userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };
            foreach (var role in userRole)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            return new TokenResponse
            {
                Token = tokenString,
                RefreshToken = null,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ResponseBase<Users>> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return new ResponseBase<Users>(null, "User not found", HttpStatusCode.NotFound);

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isPasswordValid)
                    return new ResponseBase<Users>(null, "Password is incorrect", HttpStatusCode.BadRequest);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return new ResponseBase<Users>(null, "Password update failed", HttpStatusCode.InternalServerError)
                    {
                        Errors = errors
                    };
                }

                return new ResponseBase<Users>(user, "Password updated successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Users>(null, "An error occurred while updating password", HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string> { ex.Message, ex.InnerException?.Message, ex.StackTrace }
                };
            }
        }

    }
}
