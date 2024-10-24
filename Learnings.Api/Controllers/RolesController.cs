using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Learnings.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUserRolesService _userRolesService;

        public RolesController(IUserRolesService userRolesService)
        {
            _userRolesService = userRolesService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase<Users>>> AssignUserRoles(string email, string role)
        {
            var response = await _userRolesService.AssignUserRoles(email, role);
            return response;
        }
        [HttpPut]
        public async Task<ActionResult<ResponseBase<Users>>> UpdateUserRoles(string email, string role)
        {
            var response = await _userRolesService.UpdateUserRoles(email, role);
            return response;
        }
    }
}
