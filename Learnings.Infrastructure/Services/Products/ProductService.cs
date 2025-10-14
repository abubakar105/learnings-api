
using Learnings.Application.Dtos.FilterationDto;
using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.CurrentLoggedInUser;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<ProductService> _logger;
        public ProductService(
            LearningDbContext db,
            ICurrentUserService currentUser,
            UserManager<Users> userManager,
            IBlobStorageService blobStorageService,
            ILogger<ProductService> logger)
        {
            _db = db;
            _current = currentUser;
            _userManager = userManager;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        public async Task<ResponseBase<AddProductDto>> CreateProduct(AddProductDto dto)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Creating product: {Name}, Images count: {Count}",
                    dto.Name, dto.ImageUrls?.Count ?? 0);

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
                    _logger.LogInformation("Uploading {Count} images to blob storage", dto.ImageUrls.Count);

                    var productImages = new List<ProductImage>();

                    for (int i = 0; i < dto.ImageUrls.Count; i++)
                    {
                        var file = dto.ImageUrls[i];

                        try
                        {
                            var imageUrl = await _blobStorageService.UploadFileAsync(file, "product-images");

                            _logger.LogInformation("Image {Index} uploaded: {Url}", i, imageUrl);

                            productImages.Add(new ProductImage
                            {
                                ImageId = Guid.NewGuid(),
                                ProductId = product.ProductId,
                                Url = imageUrl,  // Blob Storage URL
                                SortOrder = i,
                                AltText = $"{dto.Name} - Image {i + 1}",
                                CreatedBy = _current.UserId,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedBy = _current.UserId
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to upload image {Index}", i);
                            throw new Exception($"Failed to upload image {i + 1}: {ex.Message}");
                        }
                    }

                    product.ProductImages = productImages;
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Product created successfully: {ProductId}", product.ProductId);

                dto.ProductId = product.ProductId;
                return new ResponseBase<AddProductDto>(dto, "Product created successfully.", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
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
        //public async Task<ResponseBase<AddProductDto>> CreateProduct(AddProductDto dto)
        //{
        //    await using var transaction = await _db.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var product = new Product
        //        {
        //            ProductId = Guid.NewGuid(),
        //            Name = dto.Name,
        //            SKU = dto.SKU,
        //            Description = dto.Description,
        //            Price = dto.Price,
        //            IsActive = dto.IsActive,
        //            CreatedBy = _current.UserId,
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedBy = _current.UserId

        //        };

        //        if (dto.CategoryIds?.Any() == true)
        //        {
        //            var categoryIds = dto.CategoryIds
        //                                .Select(c => c.childCategoryId)
        //                                .Distinct()
        //                                .ToList();

        //            var categories = await _db.Categories
        //                                      .Where(c => categoryIds.Contains(c.CategoryId))
        //                                      .ToListAsync();

        //            foreach (var cat in categories)
        //            {
        //                product.Categories.Add(cat);
        //            }
        //        }

        //        if (dto.Attributes?.Any() == true)
        //        {
        //            product.ProductAttributes = dto.Attributes
        //                .Select(a => new ProductAttribute
        //                {
        //                    ProductId = product.ProductId,
        //                    ProductsAttributeId = a.AttributeTypeId,
        //                    Value = a.Value,
        //                    CreatedBy = _current.UserId,
        //                    UpdatedBy = _current.UserId,
        //                    CreatedAt = DateTime.UtcNow
        //                })
        //                .ToList();
        //        }

        //        if (dto.ImageUrls?.Any() == true)
        //        {
        //            product.ProductImages = dto.ImageUrls
        //                .Select((url, idx) => new ProductImage
        //                {
        //                    ImageId = Guid.NewGuid(),
        //                    ProductId = product.ProductId,
        //                    Url = "",
        //                    SortOrder = idx,
        //                    AltText =  "",
        //                    CreatedBy = _current.UserId,
        //                    CreatedAt = DateTime.UtcNow,
        //                    UpdatedBy = _current.UserId
        //                })
        //                .ToList();
        //        }

        //        _db.Products.Add(product);
        //        await _db.SaveChangesAsync();

        //        await transaction.CommitAsync();

        //        dto.ProductId = product.ProductId;
        //        return new ResponseBase<AddProductDto>(dto, "Product created successfully.", HttpStatusCode.Created);
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();

        //        var resp = new ResponseBase<AddProductDto>(
        //            null,
        //            "Error creating product.",
        //            HttpStatusCode.InternalServerError
        //        );
        //        resp.Errors.Add(ex.Message);
        //        if (ex.InnerException != null)
        //            resp.Errors.Add(ex.InnerException.Message);

        //        return resp;
        //    }
        //}

        public async Task<ResponseBase<List<AllProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _db.Products
                    .Include(p => p.Categories)          // assuming your Product→Category many-to-many nav
                    .Include(p => p.ProductAttributes)
                    .Include(p => p.ProductImages)
                    .ToListAsync();

                var dtos = products.Select(p => new AllProductDto
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
                                   }).ToList()
                                   ,

                    // sort by SortOrder and project Url strings
                    ImageUrls = p.ProductImages
                                   .OrderBy(pi => pi.SortOrder)
                                   .Select(pi => pi.Url)
                                   .ToList()
                })
                .ToList();

                return new ResponseBase<List<AllProductDto>>(
                    dtos,
                    "Products retrieved successfully.",
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<List<AllProductDto>>(
                    null,
                    "Error retrieving products.",
                    HttpStatusCode.InternalServerError
                );
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }
        public async Task<ResponseBase<PagedResult<AllProductDto>>> GetProductsAsync(ProductFilterDto filter)
        {
            try
            {
                IQueryable<Product> query = _db.Products
                    .Include(p => p.Categories)
                    .Include(p => p.ProductAttributes)
                    .Include(p => p.ProductImages);

                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var term = filter.Search.Trim().ToLower();
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(term) || p.Description.ToLower().Contains(term));
                }

                if (filter.CategoryIds?.Any() == true)
                {
                    query = query.Where(p => p.Categories.Any(
                        c => filter.CategoryIds.Contains(c.CategoryId)));
                }

                if (!string.IsNullOrWhiteSpace(filter.Gender))
                {
                    query = query.Where(p => p.ProductAttributes.Any(
                        c => c.Value.ToLower().Contains(filter.Gender)));
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }

                var total = await query.CountAsync();

                query = filter.SortBy switch
                {
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "price_asc" => query.OrderBy(p => p.Price),
                    "newest" => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.Name)
                };

                var skip = (filter.Page - 1) * filter.PageSize;

                var products = await query
                    .Skip(skip)
                    .Take(filter.PageSize)
                    .Select(p => new AllProductDto
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
                            .ToList()
                            ,
                        ImageUrls = p.ProductImages
                            .OrderBy(pi => pi.SortOrder)
                            .Select(pi => pi.Url)
                            .ToList()
                    })
                    .ToListAsync();

                return new ResponseBase<PagedResult<AllProductDto>>(
                    new PagedResult<AllProductDto>
                    {
                        TotalCount = total,
                        Items = products
                    },
                    "Product retrieved successfully.",
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<PagedResult<AllProductDto>>(
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

        public async Task<ResponseBase<AllProductDto>> GetSingleProduct(Guid productId)
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
                    return new ResponseBase<AllProductDto>(
                        null,
                        "Product not found.",
                        HttpStatusCode.NotFound
                    );
                }

                // Map to your DTO
                var dto = new AllProductDto
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
                return new ResponseBase<AllProductDto>(
                    dto,
                    "Product retrieved successfully.",
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<AllProductDto>(
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

