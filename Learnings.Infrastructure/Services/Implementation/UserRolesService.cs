using Learnings.Application.Dtos;
using Learnings.Application.Repositories.Interface;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Domain.Share;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
                //await CreateRoles();
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ResponseBase<Users>(null, "User not found", HttpStatusCode.NotFound);
                }
                if (IsValidRole(role))
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
        public bool IsValidRole(string role)
        {
            var validRoles = new List<string> { Roles.SuperAdmin.ToLower(), Roles.Admin.ToLower(), Roles.User.ToLower() };
            return validRoles.Contains(role.ToLower());
        }

        public async Task<ResponseBase<Users>> UpdateUserRoles(string email, string role)
        {
            ResponseBase<Users> response;
            try
            {
                //await CreateRoles();
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ResponseBase<Users>(null, "User not found", HttpStatusCode.NotFound);
                }
                if (IsValidRole(role))
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

        //private async Task CreateRoles()
        //{
        //    var roles = new List<string> { Roles.SuperAdmin, Roles.Admin, Roles.User };

        //    foreach (var role in roles)
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(role));
        //        }
        //    }
        //}
    }
}
