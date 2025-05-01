using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class Cart : AuditableEntity
    {
        [Key]
        public Guid CartId { get; set; }

        [Required]
        public string UserId { get; set; }

        // nav props
        public Users User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();
    }
}
