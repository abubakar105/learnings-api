using Learnings.Application.Dtos;
using Learnings.Application.Repositories.Interface;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Domain.Share;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Net;

namespace Learnings.Infrastructure.Services.Implementation
{
    public class UserRolesService : IUserRolesService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesService(IUserRepository userRepository, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager; 
        }

        public async Task<ResponseBase<Users>> AssignUserRoles(string email,string role)
        {
            ResponseBase<Users> response;
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ResponseBase<Users>(null, "User not found", HttpStatusCode.NotFound);
                }

                var isValidRole = GetValidRole(role);

                if (!string.IsNullOrEmpty(isValidRole))
                {
                    var result = await _userManager.AddToRoleAsync(user, role);

                    if (result.Succeeded)
                    {
                        return new ResponseBase<Users>(null, "User role assigned successfully", HttpStatusCode.OK);
                    }
                    else
                    {
                        return new ResponseBase<Users>(null, "Failed to assign role", HttpStatusCode.BadRequest)
                        {
                            Errors = result.Errors.Select(e => e.Description).ToList()
                        };
                    }
                }
                else
                {
                    return new ResponseBase<Users>(null, "Invalid User Role", HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while assigning roles to the user.";
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

        public async Task<ResponseBase<Users>> DeleteUserRoles(string userEmail, string role)
        {
            ResponseBase<Users> response;
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    return new ResponseBase<Users>(null, "User not found", HttpStatusCode.NotFound);
                }

                var isValidRole = GetValidRole(role);

                if (!string.IsNullOrEmpty(isValidRole))
                {
                    var getUserRoles = await _userManager.GetRolesAsync(user);

                    var roleExists = getUserRoles.Where(x => x.Equals(isValidRole, StringComparison.OrdinalIgnoreCase));
                    if(roleExists == null)
                    {
                        return new ResponseBase<Users>(null, role + " role already exists", HttpStatusCode.OK);
                    }

                    var result = await _userManager.RemoveFromRoleAsync(user, role);

                    if (result.Succeeded)
                    {
                        return new ResponseBase<Users>(null, role +" role removed successfully", HttpStatusCode.OK);
                    }
                    else
                    {
                        return new ResponseBase<Users>(null, "Failed to remove role", HttpStatusCode.BadRequest)
                        {
                            Errors = result.Errors.Select(e => e.Description).ToList()
                        };
                    }
                }
                else
                {
                    return new ResponseBase<Users>(null, "Invalid User Role "+role, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while assigning roles to the user.";
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

        public async Task<ResponseBase<UserWithRolesDto>> GetUserRoles(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    return new ResponseBase<UserWithRolesDto>(null, "User not found", HttpStatusCode.NotFound);
                }

                var getUserRoles = await _userManager.GetRolesAsync(user);

                var userWithRoles = new UserWithRolesDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = getUserRoles.ToList()
                };

                return new ResponseBase<UserWithRolesDto>(userWithRoles, "User roles retrieved successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while retrieving roles for the user.";
                return new ResponseBase<UserWithRolesDto>(null, errorMessage, HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string>
                    {
                        ex.Message,
                        ex.InnerException?.Message,
                        ex.StackTrace
                    }
                };
            }
        }



        public string? GetValidRole(string role)
        {
            return typeof(Roles).GetFields().Select(f => f.GetValue(null)?.ToString())
                                .FirstOrDefault(r => r?.Equals(role, StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}
