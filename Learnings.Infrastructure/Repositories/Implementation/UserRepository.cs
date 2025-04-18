using Learnings.Application.Dtos;
using Learnings.Application.Repositories.Interface;
using Learnings.Application.ResponseBase;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Learnings.Infrastrcuture.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly LearningDbContext _context;

        public UserRepository(LearningDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseBase<Users>> GetUserByIdAsync(int id)
        {
            ResponseBase<Users> response = null;
            try
            {
                //var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                //var user = null;
                return response = new ResponseBase<Users>(null, "User", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "Could not find user";
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

        public async Task<ResponseBase<List<Users>>> GetAllUsersAsync()
        {
            ResponseBase<List<Users>> response = null;
            try
            {

                var userList = await _context.Users.ToListAsync();
                return response = new ResponseBase<List<Users>>(userList, "User found Successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = "Could not found user";
                response = new ResponseBase<List<Users>>(null, errorMessage, HttpStatusCode.InternalServerError)
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

        public async Task<ResponseBase<Users>> AddUserAsync(Users user)
        {
            ResponseBase<Users> response = null;
            try
            {

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return response = new ResponseBase<Users>(user, "User Created Successfully", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                var errorMessage = "Could not create user";
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

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUserAsync(Users user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
