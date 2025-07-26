using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Services.Interface
{
    public interface IReviewService
    {
        Task<ResponseBase<ReviewDto>> CreateReviewAsync(AddReviewDto dto);
        Task<ResponseBase<ReviewDto>> GetReviewByIdAsync(Guid reviewId);
        Task<ResponseBase<List<ReviewDto>>> GetAllReviewsAsync(Guid productId);
        Task<ResponseBase<ReviewDto>> UpdateReviewAsync(UpdateReviewDto dto);
        Task<ResponseBase<bool>> DeleteReviewAsync(Guid reviewId);
    }
}
