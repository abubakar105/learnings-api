using Learnings.Application.Dtos;
using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Learnings.Application.Services.Interface
{
    public interface IUserRolesService
    {
        Task<ResponseBase<Users>> AssignUserRoles(AssignRole assignRole);
        Task<ResponseBase<Users>> DeleteUserRoles(AssignRole removeRole);
        Task<ResponseBase<UserWithRolesDto>> GetUserRoles(Email email);
        Task<ResponseBase<List<IdentityRole>>> GetUserRoles();
        Task<ResponseBase<List<PermissionsDto>>> GetPermissionsForRole(int roleId);
        Task<ResponseBase<List<IdentityRole>>> SearchRoles(string searchRole);
        Task<ResponseBase<AssignPermissionsToRoleDTO>> RemovePermissionFromRole(AssignPermissionsToRoleDTO assignPermissionsToRole);
        Task<ResponseBase<IdentityRole>> CreateRole(RoleDto roleDto);
        Task<ResponseBase<IdentityRole>> GetRoleById(string roleDto);
        Task<ResponseBase<IdentityRole>> UpdateRole(RoleDto roleDto);
        Task<ResponseBase<IdentityRole>> DeleteRole(string roleId);
        Task<ResponseBase<AssignPermissionsToRoleDTO>> assignPermissionsToRole(AssignPermissionsToRoleDTO assignPermissionsToRoleDTO);

    }
}
