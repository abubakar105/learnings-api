using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Products
{
    public interface IProductsLookUpAttributeService
    {
        Task<ResponseBase<List<ProductsLookUpAttributeDto>>> GetAllAsync();
        Task<ResponseBase<ProductsLookUpAttributeDto>> GetByIdAsync(Guid id);
        Task<ResponseBase<ProductsLookUpAttributeDto>> CreateAsync(ProductsLookUpAttributeDto dto);
        Task<ResponseBase<ProductsLookUpAttributeDto>> UpdateAsync(Guid id, ProductsLookUpAttributeDto dto);
        Task<ResponseBase<string>> DeleteAsync(Guid id);
    }
}
