using Learnings.Application.Dtos;
using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services.Implementation
{
    public class PermissionsService : IPermissionsService
    {
        private readonly LearningDbContext _context;
        private readonly UserManager<Users> _userManager;


        public PermissionsService(UserManager<Users> userManager, LearningDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ResponseBase<PermissionsDto>> CreatePermission(PermissionsDto permissionsDto)
        {
            try
            {
                var permission = new Permissions
                {
                    PermissionName = permissionsDto.PermissionName,
                    PermissionDescription=permissionsDto.PermissionDescription,
                };

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                permissionsDto.PermissionId = permission.PermissionId;

                return new ResponseBase<PermissionsDto>(permissionsDto, "Permission created successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<PermissionsDto>(null, "Error creating permission.", HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<List<PermissionsDto>>> GetAllPermissions()
        {
            try
            {
                var permissions = await _context.Permissions
                    .Select(p => new PermissionsDto
                    {
                        PermissionId = p.PermissionId,
                        PermissionName = p.PermissionName
                    })
                    .ToListAsync();

                return new ResponseBase<List<PermissionsDto>>(permissions, "Permissions retrieved successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<PermissionsDto>>(null, "Error retrieving permissions.", HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<PermissionsDto>> GetPermissionById(int id)
        {
            try
            {
                var permission = await _context.Permissions
                    .Where(p => p.PermissionId == id)
                    .Select(p => new PermissionsDto
                    {
                        PermissionId = p.PermissionId,
                        PermissionName = p.PermissionName
                    })
                    .FirstOrDefaultAsync();

                if (permission == null)
                {
                    return new ResponseBase<PermissionsDto>(null, "Permission not found.", HttpStatusCode.NotFound);
                }

                return new ResponseBase<PermissionsDto>(permission, "Permission retrieved successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<PermissionsDto>(null, "Error retrieving permission.", HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<PermissionsDto>> UpdatePermission(PermissionsDto permissionsDto)
        {
            try
            {
                var permission = await _context.Permissions.FindAsync(permissionsDto.PermissionId);
                if (permission == null)
                {
                    return new ResponseBase<PermissionsDto>(null, "Permission not found.", HttpStatusCode.NotFound);
                }

                permission.PermissionName = permissionsDto.PermissionName;

                _context.Permissions.Update(permission);
                await _context.SaveChangesAsync();

                return new ResponseBase<PermissionsDto>(permissionsDto, "Permission updated successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<PermissionsDto>(null, "Error updating permission.", HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<bool>> DeletePermission(int id)
        {
            try
            {
                var permission = await _context.Permissions.FindAsync(id);
                if (permission == null)
                {
                    return new ResponseBase<bool>(false, "Permission not found.", HttpStatusCode.NotFound);
                }

                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();

                return new ResponseBase<bool>(true, "Permission deleted successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, "Error deleting permission.", HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<List<PermissionsDto>>> GetUserPermissions(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseBase<List<PermissionsDto>>(null, "User not found.", HttpStatusCode.NotFound);
                }
                var userRoles = await _context.Roles.Where(x => x.Id==user.Id).Select(x =>x.Id).ToListAsync();
                if (userRoles.Any())
                {
                    var userPermissions = await _context.RolePermissions.Where(rp => userRoles.Contains(rp.RoleId)).ToListAsync();
                    if (userPermissions.Any())
                    {
                        var permissionIds = userPermissions.Select(up => up.PermissionId).ToList();
                        var permissions = await _context.Permissions
                            .Where(p => permissionIds.Contains(p.PermissionId))
                            .ToListAsync();
                        //var permissions = await _context.Permissions.Where(x=> userPermissions.Contains(x.PermissionId)).ToListAsync();
                    }
                }

                return new ResponseBase<List<PermissionsDto>>(null, "Permission deleted successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<PermissionsDto>> (null, "Error deleting permission.", HttpStatusCode.InternalServerError)
                {
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
