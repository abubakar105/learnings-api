using Learnings.Application.Dtos;
using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using System.Net;

namespace Learnings.Api.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;

        public RolesController(IUserRolesService userRolesService)
        {
            _userRolesService = userRolesService;
        }
        [HttpPost("Get User Roles")]
        public async Task<ActionResult<ResponseBase<UserWithRolesDto>>> GetUserRoles([FromBody] Email email)
        {
            var response = await _userRolesService.GetUserRoles(email);
            return response;
        }
        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<ResponseBase<List<RoleWithPermissionsDto>>>> GetAllRoles()
        {
            var response = await _userRolesService.GetUserRoles();
            return response;
        }
        [HttpGet("GetAllRolesNotAssigned/{roleId}")]
        public async Task<ActionResult<ResponseBase<List<IdentityRole>>>> GetAllRolesNotAssigned(string roleId)
        {
            var response = await _userRolesService.GetAdminRolesNotAssigned(roleId);
            return response;
        }
        [HttpGet("GetRoleById/{roleId}")]
        public async Task<ActionResult<ResponseBase<IdentityRole>>> GetRoleById(string roleId)
        {
            var response = await _userRolesService.GetRoleById(roleId);
            return response;
        }
        [HttpGet("GetPermissionsForRole/{roleId}")]
        public async Task<ActionResult<ResponseBase<List<PermissionsDto>>>> GetPermissionsForRole(int roleId)
        {
            var response = await _userRolesService.GetPermissionsForRole(roleId);
            return response;
        }

        [HttpPost("RemovePermissionFromRole")]
        public async Task<ActionResult<ResponseBase<AssignPermissionsToRoleDTO>>> RemovePermissionFromRole([FromBody] AssignPermissionsToRoleDTO assignPermissionsToRole)
        {
            var response = await _userRolesService.RemovePermissionFromRole(assignPermissionsToRole);
            return response;
        }

        [HttpPost("CreateRole")]
        public async Task<ActionResult<ResponseBase<IdentityRole>>> CreateRole([FromBody] RoleDto roleDto)
        {
            var response = await _userRolesService.CreateRole(roleDto);
            return response;
        }


        [HttpPut("UpdateRole")]
        public async Task<ActionResult<ResponseBase<IdentityRole>>> UpdateRole([FromBody] RoleDto roleDto)
        {
            var response = await _userRolesService.UpdateRole(roleDto);
            return response;
        }
        [HttpDelete("DeleteRole/{roleId}")]
        public async Task<ActionResult<ResponseBase<IdentityRole>>> DeleteRole(string roleId)
        {
            var response = await _userRolesService.DeleteRole(roleId);
            return response;
        }
        [HttpGet("SearchRoles")]
        public async Task<ActionResult<ResponseBase<List<IdentityRole>>>> SearchRoles([FromQuery] string searchRole)
        {
            var response = await _userRolesService.SearchRoles(searchRole);
            return response;
        }

        [HttpPost("AssignPermissionsToRole")]
        public async Task<ActionResult<ResponseBase<AssignPermissionsToRoleDTO>>> AssignPermissionsToRole([FromBody] AssignPermissionsToRoleDTO assignPermissionsToRole)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase<AssignPermissionsToRoleDTO>(null, "Invalid data.", HttpStatusCode.NotFound)
                {
                    Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
                };
            }
            var response = await _userRolesService.assignPermissionsToRole(assignPermissionsToRole);
            return response;
        }

        [HttpPost("AssignOrAddRoleToUser")]
        public async Task<ActionResult<ResponseBase<Users>>> AssignUserRoles([FromBody] AssignRole assignRole)
        {
            var response = await _userRolesService.AssignUserRoles(assignRole);
            return response;
        }

        [HttpPost("DeleteUserRole")]
        public async Task<ActionResult<ResponseBase<Users>>> DeleteUserRoles([FromBody] AssignRole removeRole)
        {
            var response = await _userRolesService.DeleteUserRoles(removeRole);
            return response;
        }
        [HttpGet("GetAllNotAssignedPermissionsForRole/{roleId}")]
        public async Task<ActionResult<ResponseBase<List<PermissionsDto>>>> AllNotAssignedPermissionsOfRole(string roleId)
        {
            var response = await _userRolesService.AllNotAssignedPermissionsOfRole(roleId);
            return response;
        }
        [HttpPut("UpdateRoleWithPermissions/{roleId}")]
        public async Task<ActionResult<ResponseBase<List<PermissionsDto>>>> UpdateRoleWithPermissions([FromBody] UpdateRoleWithPermissionsDTO updateRoleWithPermissionsDTO)
        {
            var response = await _userRolesService.UpdateRoleWithPermissions(updateRoleWithPermissionsDTO);
            return response;
        }
    }
}
