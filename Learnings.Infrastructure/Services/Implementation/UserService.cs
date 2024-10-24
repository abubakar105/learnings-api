using Learnings.Application.Dtos;
using Learnings.Application.Repositories.Interface;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Learnings.Infrastructure.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<Users> _userManager;

        public UserService(IUserRepository userRepository, UserManager<Users> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;

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
                var user = new Users
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName= userDto.UserName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    PasswordHash=userDto.Password
                };
                var res = await _userManager.CreateAsync(user);

                if (res.Succeeded)
                {
                    return new ResponseBase<Users>(user, "User created successfully", HttpStatusCode.OK);
                }
                else
                {
                    response = new ResponseBase<Users>(null, "Could not User", HttpStatusCode.BadRequest)
                    {
                        Errors = res.Errors.Select(e => e.Description).ToList()
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

        //public async Task<ResponseBase<Users>> AddUserAsyncIdentity(Users user)
        //{
        //    ResponseBase<Users> response = null;
        //    try
        //    {
        //        var res = await _userManager.CreateAsync(user);
        //        return response = new response<Users>(res, "OK", HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorMessage = "Could not found user";
        //        response = new ResponseBase<List<Users>>(null, errorMessage, HttpStatusCode.InternalServerError)
        //        {
        //            Errors = new List<string>
        //            {
        //                ex.Message,
        //                ex.InnerException?.Message,
        //                ex.StackTrace
        //            }
        //        };
        //        return response;
        //    }

        //}
    }
}
