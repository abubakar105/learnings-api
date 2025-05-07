using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.CurrentLoggedInUser;
using Learnings.Application.Services.Products;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services.Products
{
    public class CategoryService : ICategoryService
    {
        private readonly LearningDbContext _db;
        private readonly ICurrentUserService _current;

        public CategoryService(
            LearningDbContext db,
            ICurrentUserService currentUser)
        {
            _db = db;
            _current = currentUser;
        }

        public async Task<ResponseBase<List<CategoryDto>>> GetAllAsync()
        {
            try
            {
                var list = await _db.Categories
                    .Where(x => !x.IsDeleted)
                    .Select(x => new CategoryDto
                    {
                        CategoryId = x.CategoryId,
                        Name = x.Name,
                        ParentCategoryId = x.ParentCategoryId,
                        IsActive = x.IsActive
                    })
                    .ToListAsync();

                if (list == null || !list.Any())
                    return new ResponseBase<List<CategoryDto>>(
                        null, "No categories found.", HttpStatusCode.NotFound);

                return new ResponseBase<List<CategoryDto>>(
                    list, "Categories retrieved.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<List<CategoryDto>>(
                    null, "Error retrieving categories.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<CategoryDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var ent = await _db.Categories
                    .FirstOrDefaultAsync(x => x.CategoryId == id && !x.IsDeleted);

                if (ent == null)
                    return new ResponseBase<CategoryDto>(
                        null, "Category not found.", HttpStatusCode.NotFound);

                var dto = new CategoryDto
                {
                    CategoryId = ent.CategoryId,
                    Name = ent.Name,
                    ParentCategoryId = ent.ParentCategoryId,
                    IsActive = ent.IsActive
                };

                return new ResponseBase<CategoryDto>(
                    dto, "Category retrieved.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<CategoryDto>(
                    null, "Error retrieving category.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<CategoryDto>> CreateAsync(CategoryDto dto)
        {
            try
            {
                // optional: validate parent exists
                if (dto.ParentCategoryId.HasValue)
                {
                    var parentExists = await _db.Categories
                        .AnyAsync(x => x.CategoryId == dto.ParentCategoryId && !x.IsDeleted);
                    if (!parentExists)
                        return new ResponseBase<CategoryDto>(
                            null, "Parent category not found.", HttpStatusCode.BadRequest);
                }

                // check duplicate name at same level
                if (await _db.Categories.AnyAsync(x =>
                        x.Name == dto.Name
                        && x.ParentCategoryId == dto.ParentCategoryId
                        && !x.IsDeleted))
                {
                    return new ResponseBase<CategoryDto>(
                        null, "A sibling category with that name already exists.",
                        HttpStatusCode.Conflict);
                }

                var ent = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = dto.Name,
                    ParentCategoryId = dto.ParentCategoryId,
                    IsActive = dto.IsActive,
                    CreatedBy = _current.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedBy = _current.UserId,
                };

                _db.Categories.Add(ent);
                await _db.SaveChangesAsync();

                dto.CategoryId = ent.CategoryId;
                return new ResponseBase<CategoryDto>(
                    dto, "Category created.", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<CategoryDto>(
                    null, "Error creating category.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<CategoryDto>> UpdateAsync(Guid id, CategoryDto dto)
        {
            try
            {
                var ent = await _db.Categories
                    .FirstOrDefaultAsync(x => x.CategoryId == id && !x.IsDeleted);

                if (ent == null)
                    return new ResponseBase<CategoryDto>(
                        null, "Category not found.", HttpStatusCode.NotFound);

                if (dto.ParentCategoryId == id)
                    return new ResponseBase<CategoryDto>(
                        null, "A category cannot be its own parent.", HttpStatusCode.BadRequest);

                // optional: validate new parent exists
                if (dto.ParentCategoryId.HasValue)
                {
                    var parentExists = await _db.Categories
                        .AnyAsync(x => x.CategoryId == dto.ParentCategoryId && !x.IsDeleted);
                    if (!parentExists)
                        return new ResponseBase<CategoryDto>(
                            null, "Parent category not found.", HttpStatusCode.BadRequest);
                }

                // check duplicate name at same level
                if (await _db.Categories.AnyAsync(x =>
                        x.Name == dto.Name
                        && x.ParentCategoryId == dto.ParentCategoryId
                        && x.CategoryId != id
                        && !x.IsDeleted))
                {
                    return new ResponseBase<CategoryDto>(
                        null, "A sibling category with that name already exists.",
                        HttpStatusCode.Conflict);
                }

                ent.Name = dto.Name;
                ent.ParentCategoryId = dto.ParentCategoryId;
                ent.IsActive = dto.IsActive;
                ent.UpdatedBy = _current.UserId;
                ent.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new ResponseBase<CategoryDto>(
                    dto, "Category updated.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<CategoryDto>(
                    null, "Error updating category.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<string>> DeleteAsync(Guid id)
        {
            try
            {
                var ent = await _db.Categories
                    .FirstOrDefaultAsync(x => x.CategoryId == id && !x.IsDeleted);

                if (ent == null)
                    return new ResponseBase<string>(
                        null, "Category not found.", HttpStatusCode.NotFound);

                ent.IsDeleted = true;
                ent.UpdatedBy = _current.UserId;
                ent.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                return new ResponseBase<string>(
                    "Deleted", "Category deleted.", HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<string>(
                    null, "Error deleting category.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }
    }
}
