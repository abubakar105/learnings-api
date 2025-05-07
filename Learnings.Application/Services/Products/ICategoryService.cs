using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Products
{
    public interface ICategoryService
    {
        Task<ResponseBase<List<CategoryDto>>> GetAllAsync();
        Task<ResponseBase<CategoryDto>> GetByIdAsync(Guid id);
        Task<ResponseBase<CategoryDto>> CreateAsync(CategoryDto dto);
        Task<ResponseBase<CategoryDto>> UpdateAsync(Guid id, CategoryDto dto);
        Task<ResponseBase<string>> DeleteAsync(Guid id);
    }
}
