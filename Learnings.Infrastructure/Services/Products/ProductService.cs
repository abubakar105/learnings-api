using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.CurrentLoggedInUser;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly LearningDbContext _db;
        private readonly ICurrentUserService _current;
        private readonly UserManager<Users> _userManager;

        public ProductService(
            LearningDbContext db,
            ICurrentUserService currentUser,
            UserManager<Users> userManager)
        {
            _db = db;
            _current = currentUser;
            _userManager = userManager;
        }

        public async Task<ResponseBase<AddProductDto>> CreateProduct(AddProductDto dto)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var product = new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = dto.Name,
                    SKU = dto.SKU,
                    Description = dto.Description,
                    Price = dto.Price,
                    IsActive = dto.IsActive,
                    CreatedBy = _current.UserId, 
                    CreatedAt = DateTime.UtcNow,
                    UpdatedBy = _current.UserId

                };

                if (dto.CategoryIds?.Any() == true)
                {
                    var categoryIds = dto.CategoryIds
                                        .Select(c => c.childCategoryId)
                                        .Distinct()
                                        .ToList();

                    var categories = await _db.Categories
                                              .Where(c => categoryIds.Contains(c.CategoryId))
                                              .ToListAsync();

                    foreach (var cat in categories)
                    {
                        product.Categories.Add(cat);
                    }
                }

                if (dto.Attributes?.Any() == true)
                {
                    product.ProductAttributes = dto.Attributes
                        .Select(a => new ProductAttribute
                        {
                            ProductId = product.ProductId,
                            ProductsAttributeId = a.AttributeTypeId,
                            Value = a.Value,
                            CreatedBy = _current.UserId,
                            UpdatedBy = _current.UserId,
                            CreatedAt = DateTime.UtcNow
                        })
                        .ToList();
                }

                if (dto.ImageUrls?.Any() == true)
                {
                    product.ProductImages = dto.ImageUrls
                        .Select((url, idx) => new ProductImage
                        {
                            ImageId = Guid.NewGuid(),
                            ProductId = product.ProductId,
                            Url = url,
                            SortOrder = idx,
                            AltText = url,
                            CreatedBy = _current.UserId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedBy = _current.UserId
                        })
                        .ToList();
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                dto.ProductId = product.ProductId;
                return new ResponseBase<AddProductDto>(dto, "Product created successfully.", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var resp = new ResponseBase<AddProductDto>(
                    null,
                    "Error creating product.",
                    HttpStatusCode.InternalServerError
                );
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null)
                    resp.Errors.Add(ex.InnerException.Message);

                return resp;
            }
        }

    }
}
