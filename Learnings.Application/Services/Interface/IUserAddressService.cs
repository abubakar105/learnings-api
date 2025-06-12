using Learnings.Application.Dtos;
using Learnings.Application.ResponseBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Interface
{
    public interface IUserAddressService
    {
        Task<ResponseBase<UserAddressDto>> CreateAddress(AddUserAddressDto dto);
        Task<ResponseBase<List<UserAddressDto>>> GetAllAddresses();
        Task<ResponseBase<UserAddressDto>> GetAddressById(Guid addressId);
        Task<ResponseBase<UserAddressDto>> UpdateAddress(Guid addressId, AddUserAddressDto dto);
        Task<ResponseBase<object>> DeleteAddress(Guid addressId);
    }
}
