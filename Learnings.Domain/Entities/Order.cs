using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class Order : AuditableEntity
    {
        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; }  // e.g. “Pending”, “Paid”, “Shipped”

        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

        // nav props
        public Users User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}
