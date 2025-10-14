
using Learnings.Application.Dtos.FilterationDto;
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
        Task<ResponseBase<List<AllProductDto>>> GetAllProducts();
        Task<ResponseBase<PagedResult<AllProductDto>>> GetProductsAsync(ProductFilterDto filter);
        Task<ResponseBase<AllProductDto>> GetSingleProduct(Guid productId);

    }
}