using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Interface
{
    public interface IProductService
    {
        Task<ResponseBase<AddProductDto>> CreateProduct(AddProductDto roleDto);
        Task<ResponseBase<List<AddProductDto>>> GetAllProducts();
        Task<ResponseBase<AddProductDto>> GetSingleProduct(Guid productId);
        IQueryable<AddProductDto> GetProducts();


    }
}
