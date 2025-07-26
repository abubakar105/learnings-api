using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.CurrentLoggedInUser;
using Learnings.Application.Services.Interface.Contracts.HUBS;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Learnings.Infrastructure.Services.Products
{
    public class ReviewService : IReviewService
    {
        private readonly LearningDbContext _db;
        private readonly ICurrentUserService _current;
        private readonly IReviewNotificationService _notifier;

        public ReviewService(
            LearningDbContext db,
            ICurrentUserService current,
            IReviewNotificationService notifier)
        {
            _db = db;
            _current = current;
            _notifier = notifier;
        }

        public async Task<ResponseBase<ReviewDto>> CreateReviewAsync(AddReviewDto dto)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = _current.UserId;
                var userName = _current.UserName;
                var entity = new Review
                {
                    ReviewId = Guid.NewGuid(),
                    ProductId = dto.ProductId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Title = dto.Title,
                    Body = dto.Body,
                    ParentReviewId = dto.ParentReviewId,
                    CreatedBy = userId,
                    CreatedByName = userName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Reviews.Add(entity);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                var resultDto = MapToDto(entity);
                await _notifier.NotifyNewReviewAsync(resultDto);

                return new ResponseBase<ReviewDto>(
                    resultDto, "Review created successfully.", System.Net.HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ErrorResponse<ReviewDto>("Error creating review.", ex);
            }
        }

        public async Task<ResponseBase<List<ReviewDto>>> GetAllReviewsAsync(Guid productId)
        {
            var list = await _db.Reviews
                .AsNoTracking()
                .Where(r => !r.IsDeleted && r.ProductId == productId)
                .ToListAsync();

            var dtos = list.Select(MapToDto).ToList();
            return new ResponseBase<List<ReviewDto>>(
                dtos, "Reviews retrieved successfully.", System.Net.HttpStatusCode.OK);
        }

        public async Task<ResponseBase<ReviewDto>> GetReviewByIdAsync(Guid reviewId)
        {
            var entity = await _db.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId && !r.IsDeleted);

            if (entity == null)
                return new ResponseBase<ReviewDto>(
                    null, "Review not found.", System.Net.HttpStatusCode.NotFound);

            return new ResponseBase<ReviewDto>(
                MapToDto(entity), "Review retrieved successfully.", System.Net.HttpStatusCode.OK);
        }

        public async Task<ResponseBase<ReviewDto>> UpdateReviewAsync(UpdateReviewDto dto)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = await _db.Reviews
                    .FirstOrDefaultAsync(r => r.ReviewId == dto.ReviewId && !r.IsDeleted);

                if (entity == null)
                    return new ResponseBase<ReviewDto>(
                        null, "Review not found.", System.Net.HttpStatusCode.NotFound);

                entity.Rating = dto.Rating;
                entity.Title = dto.Title;
                entity.Body = dto.Body;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = _current.UserId;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                var resultDto = MapToDto(entity);
                await _notifier.NotifyNewReviewAsync(resultDto);
                return new ResponseBase<ReviewDto>(
                    resultDto, "Review updated successfully.", System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ErrorResponse<ReviewDto>("Error updating review.", ex);
            }
        }

        public async Task<ResponseBase<bool>> DeleteReviewAsync(Guid reviewId)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = await _db.Reviews
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId && !r.IsDeleted);

                if (entity == null)
                    return new ResponseBase<bool>(
                        false, "Review not found.", System.Net.HttpStatusCode.NotFound);

                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = _current.UserId;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return new ResponseBase<bool>(
                    true, "Review deleted successfully.", System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ErrorResponse<bool>("Error deleting review.", ex);
            }
        }

        private static ReviewDto MapToDto(Review e) => new()
        {
            ReviewId = e.ReviewId,
            ProductId = e.ProductId,
            UserId = e.UserId,
            Rating = e.Rating,
            Title = e.Title,
            Body = e.Body,
            ParentReviewId = e.ParentReviewId,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedAt = e.UpdatedAt,
            UpdatedBy = e.UpdatedBy,
            CreatedByName = e.CreatedByName
        };

        private static ResponseBase<T> ErrorResponse<T>(string message, Exception ex)
        {
            var resp = new ResponseBase<T>(default, message, System.Net.HttpStatusCode.InternalServerError);
            resp.Errors.Add(ex.Message);
            if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
            return resp;
        }
    }
}
