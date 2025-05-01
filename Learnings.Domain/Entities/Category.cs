using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class Category : AuditableEntity
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        // Self-reference for subcategories
        public Guid? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<Category> Subcategories { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
