using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class ProductImage : AuditableEntity
    {
        [Key]
        public Guid ImageId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required, MaxLength(1000)]
        public string Url { get; set; }

        public int SortOrder { get; set; } = 0;

        [MaxLength(500)]
        public string AltText { get; set; }

        // Navigation
        public Product Product { get; set; }
    }
}
