using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Domain.Entities;
using Learnings.Infrastrcuture.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Learnings.Infrastructure.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly LearningDbContext _context;

        public CartService(LearningDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseBase<CartDto>> GetCartByUserAsync(string userId)
        {
            try
            {
                var cart = await _context.Carts
                                .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                                .ThenInclude(p => p.ProductImages)
                                .FirstOrDefaultAsync(c => c.UserId == userId);

                //if (cart.CartId == Guid.Empty)
                //{
                //    _context.Carts.Add(cart);
                //    await _context.SaveChangesAsync();
                //}
                if (cart != null)
                {
                    var dto = await MapToDtoAsync(cart);
                    return new ResponseBase<CartDto>(dto, "Cart fetched successfully.", HttpStatusCode.OK);
                }
                return new ResponseBase<CartDto>(null, "Cart is Empty.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<CartDto>(null, "Error fetching cart.", HttpStatusCode.InternalServerError)
                {
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<CartDto>> AddItemAsync(string userId, AddCartItemRequest req)
        {
            try
            {
                var cart = await GetOrCreateCartAsync(userId);

                var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == req.ProductId);
                if (existing != null)
                    existing.Quantity += req.Quantity;
                else
                    cart.CartItems.Add(new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = req.ProductId,
                        Quantity = req.Quantity,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.UtcNow
                    });

                await _context.SaveChangesAsync();

                var dto = await MapToDtoAsync(cart);
                return new ResponseBase<CartDto>(dto, "Item added to cart successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<CartDto>(null, "Error adding item to cart.", HttpStatusCode.InternalServerError)
                {
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<CartDto>> UpdateItemAsync(string userId, UpdateCartItemRequest req)
        {
            try
            {
                var cart = await _context.Carts
                                .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                                .ThenInclude(p => p.ProductImages)
                                .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return new ResponseBase<CartDto>(null, "Cart not found.", HttpStatusCode.NotFound);

                var item = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == req.CartItemId);
                if (item == null)
                    return new ResponseBase<CartDto>(null, "Item not in cart.", HttpStatusCode.NotFound);

                item.Quantity = req.Quantity;
                await _context.SaveChangesAsync();

                var dto = await MapToDtoAsync(cart);
                return new ResponseBase<CartDto>(dto, "Cart item updated successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<CartDto>(null, "Error updating cart item.", HttpStatusCode.InternalServerError)
                {
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<string>> RemoveItemAsync(string userId, Guid cartItemId)
        {
            try
            {
                var cart = await _context.Carts
                                .Include(c => c.CartItems)
                                .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return new ResponseBase<string>(null, "Cart not found.", HttpStatusCode.NotFound);

                var item = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
                if (item == null)
                    return new ResponseBase<string>(null, "Item not in cart.", HttpStatusCode.NotFound);

                cart.CartItems.Remove(item);
                await _context.SaveChangesAsync();

                return new ResponseBase<string>(null, "Item removed from cart successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<string>(null, "Error removing item from cart.", HttpStatusCode.InternalServerError)
                {
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ResponseBase<string>> ClearCartAsync(string userId)
        {
            try
            {
                var cart = await _context.Carts
                                .Include(c => c.CartItems)
                                .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return new ResponseBase<string>(null, "Cart not found.", HttpStatusCode.NotFound);

                cart.CartItems.Clear();
                await _context.SaveChangesAsync();

                return new ResponseBase<string>(null, "Cart cleared successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ResponseBase<string>(null, "Error clearing cart.", HttpStatusCode.InternalServerError)
                {
                    Errors = { ex.Message }
                };
            }
        }

        private async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _context.Carts
                            .Include(c => c.CartItems)
                            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        private async Task<CartDto> MapToDtoAsync(Cart cart)
        {
            await _context.Entry(cart)
                          .Collection(c => c.CartItems)
                          .Query()
                          .Include(ci => ci.Product)
                          .ThenInclude(p => p.ProductImages)
                          .LoadAsync();

            var items = cart.CartItems.Select(ci => new CartItemDto
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Price = ci.Product.Price,
                Quantity = ci.Quantity,
                ImageUrl = ci.Product.ProductImages.FirstOrDefault()?.Url
            }).ToList();

            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = items
            };
        }
    }
}
