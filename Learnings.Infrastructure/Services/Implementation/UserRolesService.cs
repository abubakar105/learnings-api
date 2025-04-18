using Learnings.Application.Dtos;
using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.Repositories.Interface;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Domain.Share;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Learnings.Infrastructure.NewFolder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly LearningDbContext _context;

        public UserRolesService(LearningDbContext context,IUserRepository userRepository, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<ResponseBase<Users>> AssignUserRoles(AssignRole assignRole)
        {
            ResponseBase<Users> response;

            try
            {
                var appUser = await _userManager.FindByEmailAsync(assignRole.Email);

                if (appUser == null)
                {
                    var randomPassword = RandomPasswordGenerate.Generate();
                    var newUser = new Users
                    {
                        UserName = assignRole.Email,
                        Email = assignRole.Email,
                        FirstName = assignRole.FirstName,
                        LastName = assignRole.LastName,
                        PhoneNumber = assignRole.PhoneNumber
                    };

                    var createResult = await _userManager.CreateAsync(newUser, randomPassword);
                    if (!createResult.Succeeded)
                    {
                        var errs = string.Join("; ", createResult.Errors.Select(e => e.Description));
                        return new ResponseBase<Users>(null,
                            $"Could not create user: {errs}",
                            HttpStatusCode.InternalServerError);
                    }

                    appUser = newUser;
                }

                var rolesInDb = await _roleManager.Roles
            .Where(r => assignRole.RoleId.Contains(r.Id))
            .ToListAsync();

                var validRoleIds = rolesInDb.Select(r => r.Id).ToList();
                var validRoleNames = rolesInDb.Select(r => r.Name).ToList();

                var invalidIds = assignRole.RoleId.Except(validRoleIds).ToList();
                if (invalidIds.Any())
                {
                    return new ResponseBase<Users>(null,
                        $"Invalid Role IDs: {string.Join(", ", invalidIds)}",
                        HttpStatusCode.BadRequest);
                }

                var addResult = await _userManager.AddToRolesAsync(appUser, validRoleNames);
                if (!addResult.Succeeded)
                {
                    var errs = string.Join("; ", addResult.Errors.Select(e => e.Description));
                    return new ResponseBase<Users>(appUser,
                        $"Failed to assign role(s): {errs}",
                        HttpStatusCode.BadRequest);
                }

                return new ResponseBase<Users>(appUser,
                    "User role(s) assigned successfully",
                    HttpStatusCode.OK);
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

        public async Task<ResponseBase<Users>> DeleteUserRoles(AssignRole removeRole)
        {
            ResponseBase<Users> response;
            try
            {
                var user = await _userManager.FindByEmailAsync(removeRole.Email);

                if (user == null)
                {
                    return new ResponseBase<Users>(null, "User not found", HttpStatusCode.NotFound);
                }

                // Fetch all valid roles from DB using provided Role IDs
                var rolesFromDb = await _roleManager.Roles
                    .Where(r => removeRole.RoleId.Contains(r.Id))
                    .ToListAsync();

                var validRoleNames = rolesFromDb.Select(r => r.Name).ToList();
                var validRoleIds = rolesFromDb.Select(r => r.Id).ToList();

                // Determine which IDs are invalid (not found in DB)
                var invalidRoleIds = removeRole.RoleId.Except(validRoleIds).ToList();
                if (invalidRoleIds.Any())
                {
                    return new ResponseBase<Users>(null, $"Invalid Role IDs: {string.Join(", ", invalidRoleIds)}", HttpStatusCode.BadRequest);
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var rolesNotAssigned = validRoleNames
                    .Where(r => !userRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (rolesNotAssigned.Any())
                {
                    return new ResponseBase<Users>(null, $"Roles not assigned to user: {string.Join(", ", rolesNotAssigned)}", HttpStatusCode.BadRequest);
                }

                var result = await _userManager.RemoveFromRolesAsync(user, validRoleNames);

                if (result.Succeeded)
                {
                    return new ResponseBase<Users>(null, "Roles removed successfully", HttpStatusCode.OK);
                }

                return new ResponseBase<Users>(null, "Failed to remove roles", HttpStatusCode.BadRequest)
                {
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

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

        public async Task<ResponseBase<UserWithRolesDto>> GetUserRoles(Email email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email.UserEmail);

                if (user == null)
                {
                    return new ResponseBase<UserWithRolesDto>(null, "User not found", HttpStatusCode.NotFound);
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var userWithRoles = new UserWithRolesDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = userRoles.ToList()
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

        public async Task<ResponseBase<List<IdentityRole>>> GetUserRoles()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                if(roles.Count == 0)
                {
                    return new ResponseBase<List<IdentityRole>>(null, "Roles Not Found", HttpStatusCode.NotFound);

                }

                return new ResponseBase<List<IdentityRole>>(roles, "Roles retrieved successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while retrieving roles for the user.";
                return new ResponseBase<List<IdentityRole>>(null, errorMessage, HttpStatusCode.InternalServerError)
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
        public async Task<ResponseBase<IdentityRole>> CreateRole(RoleDto roleDto)
        {
            try
            {
                if (await _roleManager.RoleExistsAsync(roleDto.Name))
                {
                    return new ResponseBase<IdentityRole>(null, "Role already exists", HttpStatusCode.BadRequest);
                }

                var role = new IdentityRole { Name = roleDto.Name };
                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    return new ResponseBase<IdentityRole>(null, "Failed to create role", HttpStatusCode.BadRequest)
                    {
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                return new ResponseBase<IdentityRole>(role, "Role created successfully", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while creating the role.";
                return new ResponseBase<IdentityRole>(null, errorMessage, HttpStatusCode.InternalServerError)
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
        public async Task<ResponseBase<IdentityRole>> UpdateRole(RoleDto roleDto)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleDto.Id);

                if (role == null)
                {
                    return new ResponseBase<IdentityRole>(null, "Role not found", HttpStatusCode.NotFound);
                }

                role.Name = roleDto.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    return new ResponseBase<IdentityRole>(null, "Failed to update role", HttpStatusCode.BadRequest)
                    {
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                return new ResponseBase<IdentityRole>(role, "Role updated successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while updating the role.";
                return new ResponseBase<IdentityRole>(null, errorMessage, HttpStatusCode.InternalServerError)
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

        public async Task<ResponseBase<AssignPermissionsToRoleDTO>> assignPermissionsToRole(AssignPermissionsToRoleDTO assignPermissions)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(assignPermissions.RoleId);

                if (role == null)
                {
                    return new ResponseBase<AssignPermissionsToRoleDTO>(null, "Role not found.", HttpStatusCode.NotFound);
                }

                var validPermissions = await _context.Permissions
                    .Where(p => assignPermissions.PermissionIds.Contains(p.PermissionId))
                    .ToListAsync();

                if (validPermissions.Count != assignPermissions.PermissionIds.Count)
                {
                    return new ResponseBase<AssignPermissionsToRoleDTO>(null, "One or more permissions are invalid.", HttpStatusCode.BadRequest)
                    {
                        Errors = assignPermissions.PermissionIds
                            .Except(validPermissions.Select(p => p.PermissionId))
                            .Select(p => $"Permission ID '{p}' not found.")
                            .ToList()
                    };
                }

                foreach (var permission in validPermissions)
                {
                    if (!_context.RolePermissions.Any(rp => rp.RoleId == role.Id && rp.PermissionId == permission.PermissionId))
                    {
                        _context.RolePermissions.Add(new RolePermissions
                        {
                            RoleId = role.Id,
                            PermissionId = permission.PermissionId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return new ResponseBase<AssignPermissionsToRoleDTO>(assignPermissions, "Permissions successfully assigned to the role.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while adding permissions to the role.";
                return new ResponseBase<AssignPermissionsToRoleDTO>(null, errorMessage, HttpStatusCode.InternalServerError)
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

        public async Task<ResponseBase<IdentityRole>> GetRoleById(string roleDto)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleDto);

                if (role == null)
                {
                    return new ResponseBase<IdentityRole>(null, "Role not found.", HttpStatusCode.NotFound);
                }

                return new ResponseBase<IdentityRole>(role, "Role found successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while finding the role.";
                return new ResponseBase<IdentityRole>(null, errorMessage, HttpStatusCode.InternalServerError)
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

        public async Task<ResponseBase<List<PermissionsDto>>> GetPermissionsForRole(int roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role == null)
                {
                    return new ResponseBase<List<PermissionsDto>>(null, "Role not found.", HttpStatusCode.NotFound);
                }
                var permissions = await _context.RolePermissions.Where(x => x.PermissionId == roleId).Join(
                    _context.Permissions,
                    rp => rp.PermissionId,
                    p => p.PermissionId,
                    (rp,r) => new PermissionsDto
                        {
                            PermissionId = r.PermissionId,
                            PermissionName = r.PermissionName,
                        }
                    ).ToListAsync();

                if (permissions == null || !permissions.Any())
                {
                    return new ResponseBase<List<PermissionsDto>>(null, "No permissions found for the role.", HttpStatusCode.NotFound);
                }

                return new ResponseBase<List<PermissionsDto>>(permissions, "Permissions found successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while finding the role.";
                return new ResponseBase<List<PermissionsDto>>(null, errorMessage, HttpStatusCode.InternalServerError)
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

        public async Task<ResponseBase<AssignPermissionsToRoleDTO>> RemovePermissionFromRole(AssignPermissionsToRoleDTO assignPermissionsToRole)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(assignPermissionsToRole.RoleId);
                if (role == null)
                {
                    return new ResponseBase<AssignPermissionsToRoleDTO>(null, "Role not found.", HttpStatusCode.NotFound);
                }
                var rolePermissionsToRemove = _context.RolePermissions
                    .Where(rp => rp.RoleId == assignPermissionsToRole.RoleId && assignPermissionsToRole.PermissionIds.Contains(rp.PermissionId));

                if (!rolePermissionsToRemove.Any())
                {
                    return new ResponseBase<AssignPermissionsToRoleDTO>(null, "No matching permissions found for the role.", HttpStatusCode.NotFound);
                }

                _context.RolePermissions.RemoveRange(rolePermissionsToRemove);

                await _context.SaveChangesAsync();

                return new ResponseBase<AssignPermissionsToRoleDTO>(null, "Permissions deleted successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while removing the permissions from role.";
                return new ResponseBase<AssignPermissionsToRoleDTO>(null, errorMessage, HttpStatusCode.InternalServerError)
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

        public async Task<ResponseBase<IdentityRole>> DeleteRole(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return new ResponseBase<IdentityRole>(null, "Role not found.", HttpStatusCode.NotFound);
                }
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return new ResponseBase<IdentityRole>(null, "Role deleted successfully.", HttpStatusCode.OK);
                }
                else
                {
                    return new ResponseBase<IdentityRole>(null, "Failed to delete role.", HttpStatusCode.InternalServerError)
                    {
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while removing the permissions from role.";
                return new ResponseBase<IdentityRole>(null, errorMessage, HttpStatusCode.InternalServerError)
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

        public async Task<ResponseBase<List<IdentityRole>>> SearchRoles(string searchRole)
        {
            try
            {
                var roles = await _roleManager.Roles
                            .Where(r => r.Name.Contains(searchRole, StringComparison.OrdinalIgnoreCase))
                            .ToListAsync();

                if (roles == null || roles.Count == 0)
                {
                    return new ResponseBase<List<IdentityRole>>(null, "No roles found matching the search term.", HttpStatusCode.NotFound);
                }

                return new ResponseBase<List<IdentityRole>>(roles, "Roles retrieved successfully.", HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred while searching the role.";
                return new ResponseBase<List<IdentityRole>>(null, errorMessage, HttpStatusCode.InternalServerError)
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
    }
}
