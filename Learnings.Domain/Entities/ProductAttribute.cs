using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Learnings.Domain.Entities
{
    public class ProductAttribute : AuditableEntity
    {
        public Guid ProductId { get; set; }
        public Guid ProductsAttributeId { get; set; }

        [Required, MaxLength(500)]
        public string Value { get; set; }

        // nav props
        public Product Product { get; set; }
        public ProductsAttribute ProductsAttribute { get; set; }
    }

}
