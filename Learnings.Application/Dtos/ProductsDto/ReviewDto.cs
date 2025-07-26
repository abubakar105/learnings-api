using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnings.Application.Dtos.ProductsDto
{
    public class ReviewDto
    {
        public Guid ReviewId { get; set; }
        public Guid ProductId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Guid? ParentReviewId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedByName { get; set; }
        
    }
}
