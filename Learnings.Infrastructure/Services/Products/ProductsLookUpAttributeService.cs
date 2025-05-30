using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.CurrentLoggedInUser;
using Learnings.Application.Services.Products;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services.Products
{
    public class ProductsLookUpAttributeService : IProductsLookUpAttributeService
    {
        private readonly LearningDbContext _db;
        private readonly ICurrentUserService _current;

        public ProductsLookUpAttributeService(LearningDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _current = currentUser;
        }

        public async Task<ResponseBase<List<ProductsLookUpAttributeDto>>> GetAllAsync()
        {
            try
            {
                var list = await _db.ProductsAttribute
                    .Where(x => !x.IsDeleted)
                    .Select(x => new ProductsLookUpAttributeDto
                    {
                        ProductsAttributeId = x.ProductsAttributeId,
                        Name = x.Name
                    })
                    .ToListAsync();

                if (list == null || !list.Any())
                    return new ResponseBase<List<ProductsLookUpAttributeDto>>(
                        null, "No attributes found.", HttpStatusCode.NotFound);

                return new ResponseBase<List<ProductsLookUpAttributeDto>>(
                    list, "Attributes retrieved.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<List<ProductsLookUpAttributeDto>>(
                    null, "Error retrieving attributes.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<ProductsLookUpAttributeDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var ent = await _db.ProductsAttribute
                    .Where(x => x.ProductsAttributeId == id && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (ent == null)
                    return new ResponseBase<ProductsLookUpAttributeDto>(
                        null, "Attribute not found.", HttpStatusCode.NotFound);

                var dto = new ProductsLookUpAttributeDto
                {
                    ProductsAttributeId = ent.ProductsAttributeId,
                    Name = ent.Name
                };

                return new ResponseBase<ProductsLookUpAttributeDto>(
                    dto, "Attribute retrieved.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<ProductsLookUpAttributeDto>(
                    null, "Error retrieving attribute.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<ProductsLookUpAttributeDto>> CreateAsync(ProductsLookUpAttributeDto dto)
        {
            try
            {
                // check duplicate name
                if (await _db.ProductsAttribute.AnyAsync(x => x.Name == dto.Name && !x.IsDeleted))
                    return new ResponseBase<ProductsLookUpAttributeDto>(
                        null, "Attribute name already exists.", HttpStatusCode.Conflict);

                var ent = new ProductsAttribute
                {
                    ProductsAttributeId = Guid.NewGuid(),
                    Name = dto.Name,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _current.UserId,
                    UpdatedBy = _current.UserId,
                };

                _db.ProductsAttribute.Add(ent);
                await _db.SaveChangesAsync();

                dto.ProductsAttributeId = ent.ProductsAttributeId;
                return new ResponseBase<ProductsLookUpAttributeDto>(
                    dto, "Attribute created.", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<ProductsLookUpAttributeDto>(
                    null, "Error creating attribute.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<ProductsLookUpAttributeDto>> UpdateAsync(Guid id, ProductsLookUpAttributeDto dto)
        {
            try
            {
                var ent = await _db.ProductsAttribute
                    .FirstOrDefaultAsync(x => x.ProductsAttributeId == id && !x.IsDeleted);

                if (ent == null)
                    return new ResponseBase<ProductsLookUpAttributeDto>(
                        null, "Attribute not found.", HttpStatusCode.NotFound);

                // optional: check name conflict
                if (await _db.ProductsAttribute.AnyAsync(x => x.Name == dto.Name
                        && x.ProductsAttributeId != id && !x.IsDeleted))
                {
                    return new ResponseBase<ProductsLookUpAttributeDto>(
                        null, "Another attribute with that name exists.", HttpStatusCode.Conflict);
                }

                ent.Name = dto.Name;
                ent.UpdatedAt = DateTime.UtcNow;
                ent.UpdatedBy = _current.UserId;
                await _db.SaveChangesAsync();

                return new ResponseBase<ProductsLookUpAttributeDto>(
                    dto, "Attribute updated.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<ProductsLookUpAttributeDto>(
                    null, "Error updating attribute.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<string>> DeleteAsync(Guid id)
        {
            try
            {
                var ent = await _db.ProductsAttribute
                    .FirstOrDefaultAsync(x => x.ProductsAttributeId == id && !x.IsDeleted);

                if (ent == null)
                    return new ResponseBase<string>(
                        null, "Attribute not found.", HttpStatusCode.NotFound);

                ent.IsDeleted = true;
                ent.CreatedBy = _current.UserId;
                ent.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                return new ResponseBase<string>(
                    "Deleted", "Attribute deleted.", HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<string>(
                    null, "Error deleting attribute.", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<List<ProductsLookUpAttributesValueDto>>> GetValuesOfLookupAttribute(LookupRequestDto lookupIds)
        {
            try
            {
                var ent = await _db.ProductsAttribute
                    .Include(x => x.ProductAttributes.Where(pa => pa.ProductId == lookupIds.ProductId))
                    .Where(x => lookupIds.ProductsAttributesIds.Contains( x.ProductsAttributeId ) && !x.IsDeleted).ToListAsync();


                if (ent == null)
                    return new ResponseBase<List<ProductsLookUpAttributesValueDto>>(
                        null, "Attribute not found.", HttpStatusCode.NotFound);

                var response = ent.Select( x => new ProductsLookUpAttributesValueDto
                {
                    ProductsAttributeId = x.ProductsAttributeId,
                    Name = x.Name,
                    Values = x.ProductAttributes.Select(
                        c => new LookupsValues
                        {
                            ProductsAttributeId = c.ProductsAttributeId,
                            value = c.Value
                        }
                        ).ToList()


                }).ToList();
                

                return new ResponseBase<List<ProductsLookUpAttributesValueDto>>(
                    response, "Attribute values found successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<List<ProductsLookUpAttributesValueDto>>(
                    null, "Error getting Attribute values", HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }
    }
}
