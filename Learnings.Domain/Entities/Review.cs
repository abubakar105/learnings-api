
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Domain.Entities
{
    public class Review : AuditableEntity
    {
        [Key]
        public Guid ReviewId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        [Required, MaxLength(2000)]
        public string Body { get; set; }

        public Guid? ParentReviewId { get; set; }

        // Navigation
        public Product Product { get; set; }
        public Users User { get; set; }

        public Review ParentReview { get; set; }
        public ICollection<Review> Replies { get; set; }
            = new List<Review>();
    }
}
