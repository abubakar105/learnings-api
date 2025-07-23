using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Interface
{
    public interface ICartService
    {
        Task<ResponseBase<CartDto>> GetCartByUserAsync(string userId);
        Task<ResponseBase<CartDto>> AddItemAsync(string userId, AddCartItemRequest req);
        Task<ResponseBase<CartDto>> UpdateItemAsync(string userId, UpdateCartItemRequest req);
        Task<ResponseBase<string>> RemoveItemAsync(string userId, Guid cartItemId);
        Task<ResponseBase<string>> ClearCartAsync(string userId);
    }

}
