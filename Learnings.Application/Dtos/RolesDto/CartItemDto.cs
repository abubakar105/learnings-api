using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.RolesDto
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CartDto
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Total =>
            Items.Sum(i => i.Price * i.Quantity);
    }

    public class AddCartItemRequest
    {
        [Required] public Guid ProductId { get; set; }
        [Required][Range(1, int.MaxValue)] public int Quantity { get; set; }
    }

    public class UpdateCartItemRequest
    {
        [Required] public Guid CartItemId { get; set; }
        [Required][Range(1, int.MaxValue)] public int Quantity { get; set; }
    }

}
