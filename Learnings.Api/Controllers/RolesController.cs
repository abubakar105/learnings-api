using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Learnings.Api.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;

        public RolesController(IUserRolesService userRolesService)
        {
            _userRolesService = userRolesService;
        }
        [HttpGet]
        public async Task<ActionResult<ResponseBase<UserWithRolesDto>>> GetUserRoles(string email)
        {
            var response = await _userRolesService.GetUserRoles(email);
            return response;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase<Users>>> AssignUserRoles(string email, string role)
        {
            var response = await _userRolesService.AssignUserRoles(email, role);
            return response;
        }

        [HttpDelete]
        public async Task<ActionResult<ResponseBase<Users>>> DeleteUserRoles(string userEmail, string role)
        {
            var response = await _userRolesService.DeleteUserRoles(userEmail, role);
            return response;
        }
    }
}
