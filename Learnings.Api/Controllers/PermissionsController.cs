using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Learnings.Api.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionsService _permissionsService;

        public PermissionsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }

        [HttpPost("CreatePermission")]
        public async Task<ActionResult<ResponseBase<PermissionsDto>>> CreatePermission([FromBody] PermissionsDto permissionsDto)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase<PermissionsDto>(null, "Invalid data.", HttpStatusCode.BadRequest)
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                };
            }

            var response = await _permissionsService.CreatePermission(permissionsDto);
            return response;
        }
        [HttpGet("GetAllPermissions")]
        public async Task<ActionResult<ResponseBase<List<PermissionsDto>>>> GetAllPermissions()
        {
            var response = await _permissionsService.GetAllPermissions();
            return response;
        }
        [HttpGet("GetPermission/{id}")]
        public async Task<ActionResult<ResponseBase<PermissionsDto>>> GetPermission(int id)
        {
            var response = await _permissionsService.GetPermissionById(id);
            return response;
        }
        [HttpPut("UpdatePermission")]
        public async Task<ActionResult<ResponseBase<PermissionsDto>>> UpdatePermission([FromBody] PermissionsDto permissionsDto)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase<PermissionsDto>(null, "Invalid data.", HttpStatusCode.BadRequest)
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                };
            }

            var response = await _permissionsService.UpdatePermission(permissionsDto);
            return response;
        }
        [HttpDelete("DeletePermission/{id}")]
        public async Task<ActionResult<ResponseBase<bool>>> DeletePermission(int id)
        {
            var response = await _permissionsService.DeletePermission(id);
            return response;
        }
        [HttpGet("GetUserPermissions/{userId}")]
        public async Task<ActionResult<ResponseBase<List<PermissionsDto>>>> GetUserPermissions(string userId)
        {
            var response = await _permissionsService.GetUserPermissions(userId);
            return response;
        }

    }
}
