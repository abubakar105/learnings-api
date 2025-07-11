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

        public async Task<ResponseBase<List<AddProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _db.Products
                    .Include(p => p.Categories)          // assuming your Product→Category many-to-many nav
                    .Include(p => p.ProductAttributes)
                    .Include(p => p.ProductImages)
                    .ToListAsync();

                var dtos = products.Select(p => new AddProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SKU = p.SKU,
                    Description = p.Description,
                    Price = p.Price,
                    IsActive = p.IsActive,

                    // map each Category to your CategoriesDto
                    CategoryIds = p.Categories
                                   .Select(c => new CategoriesDto
                                   {
                                       parentCategoryId = (Guid)c.ParentCategoryId, // adjust property name if different
                                       childCategoryId = c.CategoryId
                                   })
                                   .ToList(),

                    // map each ProductAttribute to AttributeValueDto
                    Attributes = p.ProductAttributes
                                   .Select(a => new AttributeValueDto
                                   {
                                       AttributeTypeId = a.ProductsAttributeId,
                                       Value = a.Value
                                   })
                                   .ToList(),

                    // sort by SortOrder and project Url strings
                    ImageUrls = p.ProductImages
                                   .OrderBy(pi => pi.SortOrder)
                                   .Select(pi => pi.Url)
                                   .ToList()
                })
                .ToList();

                return new ResponseBase<List<AddProductDto>>(
                    dtos,
                    "Products retrieved successfully.",
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<List<AddProductDto>>(
                    null,
                    "Error retrieving products.",
                    HttpStatusCode.InternalServerError
                );
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }
        public IQueryable<AddProductDto> GetProducts()
        {
            return _db.Products
                .Include(p => p.Categories)
                .Include(p => p.ProductAttributes)
                .Include(p => p.ProductImages)
                .Select(p => new AddProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SKU = p.SKU,
                    Description = p.Description,
                    Price = p.Price,
                    IsActive = p.IsActive,
                    CategoryIds = p.Categories
                                   .Select(c => new CategoriesDto
                                   {
                                       parentCategoryId = c.ParentCategoryId!.Value,
                                       childCategoryId = c.CategoryId
                                   })
                                   .ToList(),
                    Attributes = p.ProductAttributes
                                   .Select(a => new AttributeValueDto
                                   {
                                       AttributeTypeId = a.ProductsAttributeId,
                                       Value = a.Value
                                   })
                                   .ToList(),
                    ImageUrls = p.ProductImages
                                  .OrderBy(pi => pi.SortOrder)
                                  .Select(pi => pi.Url)
                                  .ToList()
                });
        }
        public async Task<ResponseBase<AddProductDto>> GetSingleProduct(Guid productId)
        {
            try
            {
                // Fetch exactly one product (or null), including all related data
                var p = await _db.Products
                    .Include(x => x.Categories)            // if this is your direct nav to Category
                    .Include(x => x.ProductAttributes)
                    .Include(x => x.ProductImages)
                    .SingleOrDefaultAsync(x => x.ProductId == productId);

                // 404 if not found
                if (p == null)
                {
                    return new ResponseBase<AddProductDto>(
                        null,
                        "Product not found.",
                        HttpStatusCode.NotFound
                    );
                }

                // Map to your DTO
                var dto = new AddProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SKU = p.SKU,
                    Description = p.Description,
                    Price = p.Price,
                    IsActive = p.IsActive,

                    // CategoriesDto expects parentCategoryId & childCategoryId
                    CategoryIds = p.Categories
                        .Select(c => new CategoriesDto
                        {
                            parentCategoryId = c.ParentCategoryId.GetValueOrDefault(),
                            childCategoryId = c.CategoryId
                        })
                        .ToList(),

                    // AttributeValueDto expects AttributeTypeId & Value
                    Attributes = p.ProductAttributes
                        .Select(a => new AttributeValueDto
                        {
                            AttributeTypeId = a.ProductsAttributeId,  // match your FK
                            Value = a.Value
                        })
                        .ToList(),

                    // Just strings for each image URL, ordered by SortOrder
                    ImageUrls = p.ProductImages
                        .OrderBy(pi => pi.SortOrder)
                        .Select(pi => pi.Url)
                        .ToList()
                };

                // Wrap and return
                return new ResponseBase<AddProductDto>(
                    dto,
                    "Product retrieved successfully.",
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<AddProductDto>(
                    null,
                    "Error retrieving product.",
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
