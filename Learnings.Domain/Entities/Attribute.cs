using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class ProductsAttribute : AuditableEntity
    {
        [Key]
        public Guid ProductsAttributeId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }
        public ICollection<ProductAttribute> ProductAttributes { get; set; }
        = new List<ProductAttribute>();
    }
}
