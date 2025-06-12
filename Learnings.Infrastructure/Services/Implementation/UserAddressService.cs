using Learnings.Application.Dtos;
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
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services
{
    public class UserAddressService : IUserAddressService
    {
        private readonly LearningDbContext _db;
        private readonly ICurrentUserService _current;
        private readonly UserManager<Users> _userManager;

        public UserAddressService(
            LearningDbContext db,
            ICurrentUserService currentUser,
            UserManager<Users> userManager)
        {
            _db = db;
            _current = currentUser;
            _userManager = userManager;
        }

        public async Task<ResponseBase<UserAddressDto>> CreateAddress(AddUserAddressDto dto)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = _current.UserId;
                if (dto.IsDefault)
                {
                    // Unset any existing default address
                    var existingDefaults = await _db.UserAddresses
                        .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted)
                        .ToListAsync();
                    foreach (var addr in existingDefaults)
                        addr.IsDefault = false;
                }

                var entity = new UserAddress
                {
                    AddressId = Guid.NewGuid(),
                    UserId = userId,
                    FullName = dto.FullName,
                    Street = dto.Street,
                    Apartment = dto.Apartment,
                    City = dto.City,
                    State = dto.State,
                    PostalCode = dto.PostalCode,
                    Country = dto.Country,
                    PhoneNumber = dto.PhoneNumber,
                    IsDefault = dto.IsDefault,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.UserAddresses.Add(entity);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var resultDto = new UserAddressDto
                {
                    AddressId = entity.AddressId,
                    FullName = entity.FullName,
                    Street = entity.Street,
                    Apartment = entity.Apartment,
                    City = entity.City,
                    State = entity.State,
                    PostalCode = entity.PostalCode,
                    Country = entity.Country,
                    PhoneNumber = entity.PhoneNumber,
                    IsDefault = entity.IsDefault,
                    CreatedAt = entity.CreatedAt
                };

                return new ResponseBase<UserAddressDto>(
                    resultDto,
                    "Address created successfully.",
                    HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var resp = new ResponseBase<UserAddressDto>(
                    null,
                    "Error creating address.",
                    HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null)
                    resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<List<UserAddressDto>>> GetAllAddresses()
        {
            try
            {
                var userId = _current.UserId;
                var entities = await _db.UserAddresses
                    .Where(a => a.UserId == userId && !a.IsDeleted)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                var dtos = entities.Select(a => new UserAddressDto
                {
                    AddressId = a.AddressId,
                    FullName = a.FullName,
                    Street = a.Street,
                    Apartment = a.Apartment,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    PhoneNumber = a.PhoneNumber,
                    IsDefault = a.IsDefault,
                    CreatedAt = a.CreatedAt
                }).ToList();

                return new ResponseBase<List<UserAddressDto>>(
                    dtos,
                    "Addresses retrieved successfully.",
                    HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<List<UserAddressDto>>(
                    null,
                    "Error retrieving addresses.",
                    HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<UserAddressDto>> GetAddressById(Guid addressId)
        {
            try
            {
                var userId = _current.UserId;
                var a = await _db.UserAddresses
                    .FirstOrDefaultAsync(x => x.AddressId == addressId && x.UserId == userId && !x.IsDeleted);

                if (a == null)
                {
                    return new ResponseBase<UserAddressDto>(
                        null,
                        "Address not found.",
                        HttpStatusCode.NotFound);
                }

                var dto = new UserAddressDto
                {
                    AddressId = a.AddressId,
                    FullName = a.FullName,
                    Street = a.Street,
                    Apartment = a.Apartment,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    PhoneNumber = a.PhoneNumber,
                    IsDefault = a.IsDefault,
                    CreatedAt = a.CreatedAt
                };

                return new ResponseBase<UserAddressDto>(
                    dto,
                    "Address retrieved successfully.",
                    HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var resp = new ResponseBase<UserAddressDto>(
                    null,
                    "Error retrieving address.",
                    HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<UserAddressDto>> UpdateAddress(Guid addressId, AddUserAddressDto dto)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = _current.UserId;
                var entity = await _db.UserAddresses
                    .FirstOrDefaultAsync(x => x.AddressId == addressId && x.UserId == userId && !x.IsDeleted);

                if (entity == null)
                {
                    return new ResponseBase<UserAddressDto>(
                        null,
                        "Address not found.",
                        HttpStatusCode.NotFound);
                }

                if (dto.IsDefault && !entity.IsDefault)
                {
                    // Unset existing default
                    var existingDefaults = await _db.UserAddresses
                        .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted)
                        .ToListAsync();
                    foreach (var addr in existingDefaults)
                        addr.IsDefault = false;
                }

                entity.FullName = dto.FullName;
                entity.Street = dto.Street;
                entity.Apartment = dto.Apartment;
                entity.City = dto.City;
                entity.State = dto.State;
                entity.PostalCode = dto.PostalCode;
                entity.Country = dto.Country;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.IsDefault = dto.IsDefault;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var updatedDto = new UserAddressDto
                {
                    AddressId = entity.AddressId,
                    FullName = entity.FullName,
                    Street = entity.Street,
                    Apartment = entity.Apartment,
                    City = entity.City,
                    State = entity.State,
                    PostalCode = entity.PostalCode,
                    Country = entity.Country,
                    PhoneNumber = entity.PhoneNumber,
                    IsDefault = entity.IsDefault,
                    CreatedAt = entity.CreatedAt
                };

                return new ResponseBase<UserAddressDto>(
                    updatedDto,
                    "Address updated successfully.",
                    HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var resp = new ResponseBase<UserAddressDto>(
                    null,
                    "Error updating address.",
                    HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }

        public async Task<ResponseBase<object>> DeleteAddress(Guid addressId)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = _current.UserId;
                var entity = await _db.UserAddresses
                    .FirstOrDefaultAsync(x => x.AddressId == addressId && x.UserId == userId && !x.IsDeleted);

                if (entity == null)
                {
                    return new ResponseBase<object>(
                        null,
                        "Address not found.",
                        HttpStatusCode.NotFound);
                }

                entity.IsDeleted = true;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseBase<object>(
                    null,
                    "Address deleted successfully.",
                    HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var resp = new ResponseBase<object>(
                    null,
                    "Error deleting address.",
                    HttpStatusCode.InternalServerError);
                resp.Errors.Add(ex.Message);
                if (ex.InnerException != null) resp.Errors.Add(ex.InnerException.Message);
                return resp;
            }
        }
    }
}
