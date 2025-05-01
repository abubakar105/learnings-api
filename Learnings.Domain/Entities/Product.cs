using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class Product : AuditableEntity
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string SKU { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<ProductImage> ProductImages { get; set; }
       = new List<ProductImage>();
        public ICollection<ProductAttribute> ProductAttributes { get; set; }
    = new List<ProductAttribute>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
