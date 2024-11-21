using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using Learnings.Domain.Entities;

namespace Learnings.Application.Services.Interface
{
    public interface IUserRolesService
    {
        Task<ResponseBase<Users>> AssignUserRoles(string email, string role);
        Task<ResponseBase<Users>> DeleteUserRoles(string userEmail, string role);
        Task<ResponseBase<UserWithRolesDto>> GetUserRoles(string userEmail);
    }
}
