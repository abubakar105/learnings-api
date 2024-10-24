using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using Learnings.Domain.Entities;

namespace Learnings.Application.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<ResponseBase<Users>> GetUserByIdAsync(int id);
        Task<ResponseBase<List<Users>>> GetAllUsersAsync();
        Task<ResponseBase<Users>> AddUserAsync(Users user);
        Task UpdateUserAsync(Users user);
        Task DeleteUserAsync(int id);
    }
}
